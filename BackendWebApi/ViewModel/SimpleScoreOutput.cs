namespace BackendWebApi.ViewModel;

public class SimpleScoreOutput
{
    public int Id { get; set; }
    public int Points { get; set; }
    public DateTime ScoredAt { get; set; }
    public int ScorerId { get; set; }

    public SimpleScoreOutput(int id, int points, DateTime scoredAt, int scorerId)
    {
        Id = id;
        Points = points;
        ScoredAt = scoredAt;
        ScorerId = scorerId;
    }
}