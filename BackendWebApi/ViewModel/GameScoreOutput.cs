namespace BackendWebApi.ViewModel;

public class GameScoreOutput
{
    public int GameId { get; set; }
    public int TeamAId { get; set; }
    public int TeamAScore { get; set; }
    public int TeamBId { get; set; }
    public int TeamBScore { get; set; }

    public GameScoreOutput(int gameId, int teamAId, int teamAScore, int teamBId, int teamBScore)
    {
        GameId = gameId;
        TeamAId = teamAId;
        TeamAScore = teamAScore;
        TeamBId = teamBId;
        TeamBScore = teamBScore;
    }
}