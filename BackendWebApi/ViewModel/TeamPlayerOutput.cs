namespace BackendWebApi.ViewModel;

public class TeamPlayerOutput
{
    public int Id { get; set; }
    public string Name { get; set; }

    public TeamPlayerOutput(int id, string name)
    {
        Id = id;
        Name = name;
    }
}