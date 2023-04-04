using System.ComponentModel.DataAnnotations;

namespace BackendWebApi.Model;

public class Player
{
    public int Id { get; set; }
    public string Name { get; set; }
    
    [Required]
    public Team Team { get; set; }
    public int TeamId { get; set; }
}