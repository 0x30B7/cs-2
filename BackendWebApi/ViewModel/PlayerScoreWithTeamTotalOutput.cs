namespace BackendWebApi.ViewModel;

public class PlayerScoreWithTeamTotalOutput
{
    public int GameId { get; set; }
    public PlayerOutput Scorer { get; set; }
    public int Points { get; set; }
    public int TeamPointsTotal { get; set; }

    public PlayerScoreWithTeamTotalOutput(int gameId, PlayerOutput scorer, int points, int teamPointsTotal)
    {
        GameId = gameId;
        Scorer = scorer;
        Points = points;
        TeamPointsTotal = teamPointsTotal;
    }
}