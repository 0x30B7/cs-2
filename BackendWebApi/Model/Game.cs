namespace BackendWebApi.Model;

public class Game
{
    public int Id { get; set; }
    public Team TeamA { get; set; }
    public int TeamAId { get; set; }
    public Team TeamB { get; set; }
    public int TeamBId { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime? EndTime { get; set; }

    public ICollection<PlayerScore> PlayerScores { get; set; }
}