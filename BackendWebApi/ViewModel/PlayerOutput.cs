namespace BackendWebApi.ViewModel;

public class PlayerOutput
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int? TeamId { get; set; }

    public PlayerOutput(int id, string name, int? teamId)
    {
        Id = id;
        Name = name;
        TeamId = teamId;
    }
}