namespace BackendWebApi.Model;

public class Team
{
    public int Id { get; set; }
    public string Title { get; set; }
    
    public ICollection<Player> Players { get; set; }
}