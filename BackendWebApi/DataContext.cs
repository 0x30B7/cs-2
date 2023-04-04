using BackendWebApi.Model;
using Microsoft.EntityFrameworkCore;

namespace BackendWebApi;

public class DataContext : DbContext
{
    private readonly IConfiguration _configuration;
    
    public DataContext(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);

        optionsBuilder.UseNpgsql(_configuration.GetConnectionString("WebApiDatabase"));

        // optionsBuilder.UseSqlServer(
        //     "Server=.\\SQLExpress;Database=test_db;Trusted_Connection=true;TrustServerCertificate=true;"
        // );
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // modelBuilder.Entity<Team>().HasMany(t => t.Players).WithOne(p => p.Team);
    }

    public DbSet<Team> Teams { get; set; }
    public DbSet<Player> Players { get; set; }
    public DbSet<Game> Games { get; set; }
    public DbSet<PlayerScore> Scores { get; set; }
}