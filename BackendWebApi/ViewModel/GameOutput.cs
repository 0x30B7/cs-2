namespace BackendWebApi.ViewModel;

public class GameOutput
{
    public int Id { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public TeamOutput TeamA { get; set; }
    public TeamOutput TeamB { get; set; }

    public GameOutput(int id, DateTime startTime, DateTime? endTime, TeamOutput teamA, TeamOutput teamB)
    {
        Id = id;
        StartTime = startTime;
        EndTime = endTime;
        TeamA = teamA;
        TeamB = teamB;
    }
}