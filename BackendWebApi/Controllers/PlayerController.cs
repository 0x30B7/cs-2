using BackendWebApi.Model;
using BackendWebApi.ViewModel;
using Microsoft.AspNetCore.Mvc;

namespace BackendWebApi.Controllers;

public static class PlayerController
{
    public static IResult HandleGetAllPlayers(DataContext ctx)
    {
        var players = from player in ctx.Players select player;

        var transformed = new List<PlayerOutput>();

        foreach (var player in players)
        {
            transformed.Add(new PlayerOutput(player.Id, player.Name, player.TeamId));
        }

        return players.Any() ? Results.Ok(transformed) : Results.NotFound();
    }

    public static IResult HandleGetPlayerById(DataContext ctx, [FromRoute(Name = "playerId")] int playerId)
    {
        var player = ctx.Players.Find(playerId);

        if (player == null)
        {
            return Results.NotFound();
        }
        
        return Results.Ok(new PlayerOutput(player.Id, player.Name, player.TeamId));
    }

    public static IResult HandleGetPlayersByName(DataContext ctx, [FromRoute(Name = "playerName")] string playerName)
    {
        var players =
        (
            from player in ctx.Players
            where player.Name.StartsWith(playerName)
            select player
        ).ToList();

        var transformed = players.Select(player =>
            new PlayerOutput(player.Id, player.Name, player.TeamId)).ToList();

        return players.Any() ? Results.Ok(transformed) : Results.NotFound();
    }

    public static IResult HandleCreatePlayer(DataContext ctx, [FromBody] PlayerCreateInput player)
    {
        if (player.Name.Length < 2)
        {
            return Results.BadRequest("Player name must contain at least 2 characters.");
        }

        var team = ctx.Teams.Find(player.TeamId);

        if (team == null)
        {
            return Results.Conflict($"Team with id '{player.TeamId}' not found.");
        }

        var playerExist = ctx.Players.FirstOrDefault(x => x.Team.Id == player.TeamId && x.Name == player.Name);

        if (playerExist != null)
        {
            return Results.Conflict($"Player with given name already exists in given team.");
        }

        var newPlayer = new Player
        {
            Name = player.Name,
            Team = team
        };

        ctx.Players.Add(newPlayer);
        ctx.SaveChanges();

        return Results.Ok(new PlayerOutput(
            id: newPlayer.Id,
            name: newPlayer.Name,
            teamId: newPlayer.TeamId
        ));
    }

    public static IResult HandleUpdatePlayer(DataContext ctx, [FromBody] PlayerInput update)
    {
        var player = ctx.Players.Find(update.Id);

        if (player == null)
        {
            return Results.NotFound();
        }

        var updated = false;

        if (update.Name != null)
        {
            if (update.Name.Length < 2)
            {
                return Results.BadRequest("Player name must contain at least 2 characters.");
            }

            player.Name = update.Name;
            updated = true;
        }

        if (update.TeamId != null && update.TeamId != player.TeamId)
        {
            var team = ctx.Teams.Find(update.TeamId);

            if (team == null)
            {
                return Results.BadRequest("Team with the given team id does not exist.");
            }

            player.Team = team;
            updated = true;
        }

        if (updated)
        {
            ctx.SaveChanges();
        }

        return Results.Ok(new PlayerOutput(
            id: player.Id,
            name: player.Name,
            teamId: player.TeamId
        ));
    }

    public static IResult HandleDeletePlayerById(DataContext ctx, [FromRoute(Name = "playerId")] int playerId)
    {
        var player = ctx.Players.Find(playerId);

        if (player == null)
        {
            return Results.NotFound();
        }

        ctx.Players.Remove(player);
        ctx.SaveChanges();

        return Results.Ok(new PlayerOutput(
            id: player.Id,
            name: player.Name,
            teamId: player.TeamId
        ));
    }
}