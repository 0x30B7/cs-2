namespace BackendWebApi.ViewModel;

public class TeamOutput
{
    public int Id { get; set; }
    public string Title { get; set; }

    public TeamOutput(int id, string title)
    {
        Id = id;
        Title = title;
    }
}