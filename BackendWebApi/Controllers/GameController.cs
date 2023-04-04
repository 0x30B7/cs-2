using BackendWebApi.Model;
using BackendWebApi.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace BackendWebApi.Controllers;

public static class GameController
{
    public static IResult HandleGetAllGames(DataContext ctx)
    {
        var games = (
                from player in ctx.Games select player
            )
            .Include(x => x.TeamA)
            .Include(x => x.TeamB);

        var transformed = games.Select(game => new GameOutput(game.Id, game.StartTime, game.EndTime,
                new TeamOutput(game.TeamAId, game.TeamA.Title),
                new TeamOutput(game.TeamBId, game.TeamB.Title)))
            .ToList();

        return games.Any() ? Results.Ok(transformed) : Results.NotFound();
    }

    public static IResult HandleGetGameById(DataContext ctx, [FromRoute(Name = "gameId")] int gameId)
    {
        var game = ctx.Games
            .Include("TeamA")
            .Include("TeamA.Players")
            .Include("TeamB")
            .Include("TeamB.Players")
            .FirstOrDefault();

        if (game == null)
        {
            return Results.NotFound();
        }

        return Results.Ok(new GameWithPlayersOutput(game.Id, game.StartTime,
            new TeamWithPlayersOutput(game.TeamA.Id, game.TeamA.Title,
                game.TeamA.Players.Select(x => new TeamPlayerOutput(x.Id, x.Name)).ToList()),
            new TeamWithPlayersOutput(game.TeamB.Id, game.TeamB.Title,
                game.TeamB.Players.Select(x => new TeamPlayerOutput(x.Id, x.Name)).ToList())
        ));
    }

    public static IResult HandleCreateGame(DataContext ctx, [FromBody] GameCreateInput game)
    {
        var teamA = ctx.Teams.Find(game.TeamAId);

        if (teamA == null)
        {
            return Results.BadRequest($"Team A with id '{game.TeamAId}' not found.");
        }

        var teamB = ctx.Teams.Find(game.TeamBId);

        if (teamB == null)
        {
            return Results.BadRequest($"Team B with id '{game.TeamBId}' not found.");
        }

        var existingGame = ctx.Games
            .Where(x => x.EndTime == null)
            .FirstOrDefault(x =>
                x.TeamAId == teamA.Id || x.TeamBId == teamA.Id ||
                x.TeamAId == teamB.Id || x.TeamBId == teamB.Id);

        if (existingGame != null)
        {
            if ((existingGame.TeamAId == teamA.Id && existingGame.TeamBId == teamB.Id) ||
                (existingGame.TeamAId == teamB.Id && existingGame.TeamBId == teamA.Id))
            {
                return Results.Conflict("A game with these teams already exists and is still in progress.");
            }

            if (teamA.Id == existingGame.TeamAId || teamA.Id == existingGame.TeamBId)
            {
                return Results.Conflict(
                    $"Team {teamA.Title} (id {teamA.Id}) is currently in a game that is in progress.");
            }

            if (teamB.Id == existingGame.TeamAId || teamB.Id == existingGame.TeamBId)
            {
                return Results.Conflict(
                    $"Team {teamB.Title} (id {teamB.Id}) is currently in a game that is in progress.");
            }
        }

        ctx.Entry(teamA).Collection("Players").Load();

        if (!teamA.Players.Any())
        {
            return Results.BadRequest($"Team A must have at least 1 player.");
        }

        ctx.Entry(teamB).Collection("Players").Load();

        if (!teamB.Players.Any())
        {
            return Results.BadRequest($"Team B must have at least 1 player.");
        }

        var newGame = new Game()
        {
            StartTime = DateTime.UtcNow,
            TeamA = teamA,
            TeamB = teamB
        };

        ctx.Games.Add(newGame);
        ctx.SaveChanges();

        return Results.Ok(new GameWithPlayersOutput(
            id: newGame.Id,
            startTime: newGame.StartTime,
            teamA: new TeamWithPlayersOutput(teamA.Id, teamA.Title,
                teamA.Players.Select(x => new TeamPlayerOutput(x.Id, x.Name)).ToList()),
            teamB: new TeamWithPlayersOutput(teamB.Id, teamB.Title,
                teamB.Players.Select(x => new TeamPlayerOutput(x.Id, x.Name)).ToList())
        ));
    }

    public static IResult HandleMarkGameEnded(DataContext ctx, [FromRoute(Name = "gameId")] int gameId)
    {
        var game = ctx.Games
            .Include("TeamA")
            .Include("TeamB")
            .FirstOrDefault(x => x.Id == gameId);

        if (game == null)
        {
            return Results.NotFound();
        }

        if (game.EndTime != null)
        {
            return Results.Conflict($"Game with id '{gameId}' has already ended.");
        }

        game.EndTime = DateTime.UtcNow;
        ctx.SaveChanges();

        return Results.Ok(new GameOutput(game.Id, game.StartTime, game.EndTime,
            new TeamOutput(game.TeamAId, game.TeamA.Title),
            new TeamOutput(game.TeamBId, game.TeamB.Title)
        ));
    }

    public static IResult HandleDeleteGameById(DataContext ctx, [FromRoute(Name = "gameId")] int gameId)
    {
        var game = ctx.Games
            .Include("TeamA")
            .Include("TeamB")
            .FirstOrDefault(x => x.Id == gameId);

        if (game == null)
        {
            return Results.NotFound();
        }

        ctx.Games.Remove(game);
        ctx.SaveChanges();

        return Results.Ok(new GameOutput(game.Id, game.StartTime, game.EndTime,
            new TeamOutput(game.TeamAId, game.TeamA.Title),
            new TeamOutput(game.TeamBId, game.TeamB.Title)
        ));
    }
}