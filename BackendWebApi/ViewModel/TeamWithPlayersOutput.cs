namespace BackendWebApi.ViewModel;

public class TeamWithPlayersOutput
{
    public int Id { get; set; }
    public string Title { get; set; }
    public ICollection<TeamPlayerOutput> Players { get; set; }

    public TeamWithPlayersOutput(int id, string title, ICollection<TeamPlayerOutput> players)
    {
        Id = id;
        Title = title;
        Players = players;
    }
}