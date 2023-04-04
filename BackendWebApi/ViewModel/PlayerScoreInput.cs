namespace BackendWebApi.ViewModel;

public class PlayerScoreInput
{
    public int GameId { get; set; }
    public int PlayerId { get; set; }
    public int Points { get; set; }

    public PlayerScoreInput(int gameId, int playerId, int points)
    {
        GameId = gameId;
        PlayerId = playerId;
        Points = points;
    }
}