using System.ComponentModel.DataAnnotations.Schema;

namespace BackendWebApi.Model;

public class PlayerScore
{
    public int Id { get; set; }
    [Column(TypeName = "smallint")] public int Points { get; set; }
    public DateTime ScoredAt { get; set; }

    public Game Game { get; set; }
    public int GameId { get; set; }
    public Player Scorer { get; set; }
    public int ScorerId { get; set; }
}