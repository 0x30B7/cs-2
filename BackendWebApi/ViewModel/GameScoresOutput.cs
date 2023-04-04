namespace BackendWebApi.ViewModel;

public class GameScoresOutput
{
    public record SimplePlayerOutput(int Id, string ScorerName, int Points, DateTime ScoredAt);
    
    public int GameId { get; set; }
    public int TeamAId { get; set; }
    public ICollection<SimplePlayerOutput> TeamAScores { get; set; }
    public int TeamBId { get; set; }
    public ICollection<SimplePlayerOutput> TeamBScores { get; set; }

    public GameScoresOutput(int gameId, int teamAId, ICollection<SimplePlayerOutput> teamAScores, int teamBId, ICollection<SimplePlayerOutput> teamBScores)
    {
        GameId = gameId;
        TeamAId = teamAId;
        TeamAScores = teamAScores;
        TeamBId = teamBId;
        TeamBScores = teamBScores;
    }
}