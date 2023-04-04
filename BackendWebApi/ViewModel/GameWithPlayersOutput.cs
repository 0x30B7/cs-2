namespace BackendWebApi.ViewModel;

public class GameWithPlayersOutput
{
    public int Id { get; set; }
    public DateTime StartTime { get; set; }
    public TeamWithPlayersOutput TeamA { get; set; }
    public TeamWithPlayersOutput TeamB { get; set; }

    public GameWithPlayersOutput(int id, DateTime startTime, TeamWithPlayersOutput teamA, TeamWithPlayersOutput teamB)
    {
        Id = id;
        StartTime = startTime;
        TeamA = teamA;
        TeamB = teamB;
    }
}