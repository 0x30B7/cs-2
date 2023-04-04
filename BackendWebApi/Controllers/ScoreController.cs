using BackendWebApi.Model;
using BackendWebApi.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BackendWebApi.Controllers;

public class ScoreController
{
    public static IResult HandleCreateScore(DataContext ctx, [FromBody] PlayerScoreInput score)
    {
        var game = ctx.Games.Find(score.GameId);

        if (game == null)
        {
            return Results.NotFound("The game with the given id is not found.");
        }

        if (game.EndTime != null)
        {
            return Results.Conflict("The given game has already ended.");
        }

        var player = ctx.Players.Find(score.PlayerId);

        if (player == null)
        {
            return Results.NotFound("The player with the given id is not found.");
        }

        if (player.TeamId != game.TeamAId && player.TeamId != game.TeamBId)
        {
            return Results.Conflict(
                "The given player does not belong to either of the teams assigned to the given game.");
        }

        var newScore = new PlayerScore()
        {
            Points = score.Points,
            Game = game,
            Scorer = player,
            ScoredAt = DateTime.UtcNow
        };

        ctx.Scores.Add(newScore);
        ctx.SaveChanges();

        var totalPoints = ctx.Scores
            .Include("Scorer")
            .Where(x => x.GameId == game.Id && x.Scorer.TeamId == player.TeamId)
            .Sum(x => x.Points);

        return Results.Ok(new PlayerScoreWithTeamTotalOutput(game.Id,
            new PlayerOutput(player.Id, player.Name, player.TeamId),
            newScore.Points, totalPoints));
    }

    public static IResult HandleGetGameScore(DataContext ctx, [FromRoute(Name = "gameId")] int gameId)
    {
        var game = ctx.Games.Find(gameId);

        if (game == null)
        {
            return Results.NotFound();
        }

        var scoreTeamA = ctx.Scores
            .Include("Scorer")
            .Where(x => x.GameId == game.Id && x.Scorer.TeamId == game.TeamAId)
            .Sum(x => x.Points);

        var scoreTeamB = ctx.Scores
            .Include("Scorer")
            .Where(x => x.GameId == game.Id && x.Scorer.TeamId == game.TeamBId)
            .Sum(x => x.Points);

        return Results.Ok(new GameScoreOutput(game.Id, game.TeamAId, scoreTeamA, game.TeamBId, scoreTeamB));
    }

    public static IResult HandleGetGameScores(DataContext ctx, [FromRoute(Name = "gameId")] int gameId)
    {
        var game = ctx.Games
            .Include("PlayerScores")
            .Include("PlayerScores.Scorer")
            .FirstOrDefault(x => x.Id == gameId);

        if (game == null)
        {
            return Results.NotFound();
        }

        ICollection<GameScoresOutput.SimplePlayerOutput> teamAScores = new List<GameScoresOutput.SimplePlayerOutput>();
        ICollection<GameScoresOutput.SimplePlayerOutput> teamBScores = new List<GameScoresOutput.SimplePlayerOutput>();

        foreach (var score in game.PlayerScores)
        {
            if (score.Scorer.TeamId == game.TeamAId)
            {
                teamAScores.Add(new GameScoresOutput.SimplePlayerOutput(score.Id, score.Scorer.Name, score.Points,
                    score.ScoredAt));
            }
            else if (score.Scorer.TeamId == game.TeamBId) // ensure validity, despite logical intuitiveness
            {
                teamBScores.Add(new GameScoresOutput.SimplePlayerOutput(score.Id, score.Scorer.Name, score.Points,
                    score.ScoredAt));
            }
        }

        return Results.Ok(new GameScoresOutput(game.Id, game.TeamAId, teamAScores, game.TeamBId, teamBScores));
    }
    
    public static IResult HandleDeleteGameScore(DataContext ctx, [FromRoute(Name = "scoreId")] int scoreId)
    {
        var score = ctx.Scores.Find(scoreId);

        if (score == null)
        {
            return Results.NotFound();
        }

        ctx.Scores.Remove(score);
        ctx.SaveChanges();
        
        return Results.Ok(new SimpleScoreOutput(score.Id, score.Points, score.ScoredAt, score.ScorerId));
    }
}