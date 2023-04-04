using BackendWebApi.Model;
using BackendWebApi.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BackendWebApi.Controllers;

public static class TeamController
{
    public static IResult HandleGetAllTeams(DataContext ctx)
    {
        var teams = ctx.Teams.Include(x => x.Players).ToList();

        var transformed = teams.Select(team => new TeamWithPlayersOutput(team.Id, team.Title,
                team.Players.Any()
                    ? team.Players.Select(x => new TeamPlayerOutput(x.Id, x.Name)).ToList()
                    : new List<TeamPlayerOutput>()))
            .ToList();

        return teams.Any() ? Results.Ok(transformed) : Results.NotFound();
    }

    public static IResult HandleGetTeamById(DataContext ctx, [FromRoute(Name = "teamId")] int teamId)
    {
        var team = ctx.Teams.Find(teamId);

        if (team == null)
        {
            return Results.NotFound();
        }
        
        ctx.Entry(team).Collection("Players").Load();

        return Results.Ok(new TeamWithPlayersOutput(team.Id, team.Title,
            team.Players.Any()
                ? team.Players.Select(x => new TeamPlayerOutput(x.Id, x.Name)).ToList()
                : new List<TeamPlayerOutput>()));
    }

    public static IResult HandleCreateTeam(DataContext ctx, [FromBody] Team team)
    {
        if (team.Title.Length < 2)
        {
            return Results.BadRequest("Team title must contain at least 2 characters.");
        }

        ctx.Teams.Add(team);
        ctx.SaveChanges();

        return Results.Ok(new TeamOutput(team.Id, team.Title));
    }

    public static IResult HandleUpdateTeam(DataContext ctx, [FromBody] TeamInput update)
    {
        // var team = ctx.Teams.Include(x => x.Players).FirstOrDefault(x => x.Id == update.Id);
        var team = ctx.Teams.Find(update.Id);

        if (team == null)
        {
            return Results.NotFound();
        }
        
        ctx.Entry(team).Collection("Players").Load();

        var updated = false;

        if (update.Title != null)
        {
            if (update.Title.Length < 2)
            {
                return Results.BadRequest("Team title must contain at least 2 characters.");
            }

            team.Title = update.Title;
            updated = true;
        }

        if (updated)
        {
            ctx.SaveChanges();
        }

        return Results.Ok(new TeamWithPlayersOutput(team.Id, team.Title,
            team.Players.Any()
                ? team.Players.Select(x => new TeamPlayerOutput(x.Id, x.Name)).ToList()
                : new List<TeamPlayerOutput>()));
    }

    public static IResult HandleDeleteTeamById(DataContext ctx, [FromRoute(Name = "teamId")] int teamId)
    {
        var team = ctx.Teams.Find(teamId);

        if (team == null)
        {
            return Results.NotFound();
        }

        ctx.Teams.Remove(team);
        ctx.SaveChanges();

        return Results.Ok(new TeamOutput(team.Id, team.Title));
    }
}