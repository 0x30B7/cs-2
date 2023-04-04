using System.Security.Claims;
using BackendWebApi;
using BackendWebApi.Controllers;
using BackendWebApi.Middleware;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.Authority = $"https://{builder.Configuration["Auth0:Domain"]}/";
    options.Audience = builder.Configuration["Auth0:Audience"];
    options.TokenValidationParameters = new TokenValidationParameters
    {
        NameClaimType = ClaimTypes.NameIdentifier
    };
});

builder.Services.AddAuthorization(options =>
{
    var domain = $"https://{builder.Configuration["Auth0:Domain"]}/";
    foreach (var scope in new[]
             {
                 "client",
                 "admin",
                 "super-admin"
             })
    {
        options.AddPolicy(scope, policy =>
            policy.Requirements.Add(new HasScopeRequirement(scope, domain)));
    }
});

builder.Services.AddSingleton<IAuthorizationHandler, HasScopeHandler>();

builder.Services.AddDbContext<DataContext>();

var app = builder.Build();

// app.MapControllers();
app.UseCors();

app.UseAuthentication();
app.UseAuthorization();

app.UseSwagger();
app.UseSwaggerUI();

// app.UseHttpsRedirection();

app.MapGet("/api/v1/", () => "Secret!").RequireAuthorization();
app.MapGet("/callback",
    (HttpRequest ctx) =>
    {
        Console.WriteLine("callback!");
        Console.WriteLine("Query: " + ctx.QueryString);
        Console.WriteLine("Headers:");
        foreach (var header in ctx.Headers)
        {
            Console.WriteLine("  - '" + header.Key + "': '" + header.Value + "'");
        }
    });

app.MapPost("api/v1/teams", TeamController.HandleCreateTeam).RequireAuthorization("admin");
app.MapGet("api/v1/teams", TeamController.HandleGetAllTeams).RequireAuthorization("client");
app.MapGet("api/v1/teams/{teamId}", TeamController.HandleGetTeamById).RequireAuthorization("client");
app.MapMethods("api/v1/teams", new[] { "PATCH" }, TeamController.HandleUpdateTeam).RequireAuthorization("admin");
app.MapDelete("api/v1/teams/{teamId}", TeamController.HandleDeleteTeamById).RequireAuthorization("admin", "super-admin");

app.MapPost("api/v1/players", PlayerController.HandleCreatePlayer).RequireAuthorization("admin");
app.MapGet("api/v1/players", PlayerController.HandleGetAllPlayers).RequireAuthorization("client");
app.MapGet("api/v1/players/by-id/{playerId}", PlayerController.HandleGetPlayerById).RequireAuthorization("client");
app.MapGet("api/v1/players/by-name/{playerName}", PlayerController.HandleGetPlayersByName).RequireAuthorization("client");
app.MapMethods("api/v1/players", new[] { "PATCH" }, PlayerController.HandleUpdatePlayer).RequireAuthorization("admin");
app.MapDelete("api/v1/players/{playerId}", PlayerController.HandleDeletePlayerById).RequireAuthorization("super-admin");

app.MapPost("api/v1/games", GameController.HandleCreateGame).RequireAuthorization("admin");
app.MapPost("api/v1/games/{gameId}/mark-ended", GameController.HandleMarkGameEnded).RequireAuthorization("admin");
app.MapGet("api/v1/games", GameController.HandleGetAllGames).RequireAuthorization("client");
app.MapGet("api/v1/games/{gameId}", GameController.HandleGetGameById).RequireAuthorization("client");
app.MapDelete("api/v1/games/{gameId}", GameController.HandleDeleteGameById).RequireAuthorization("super-admin");

app.MapPost("api/v1/game-score", ScoreController.HandleCreateScore).RequireAuthorization("admin");
app.MapGet("api/v1/game-score/{gameId}", ScoreController.HandleGetGameScore).RequireAuthorization("client");
app.MapGet("api/v1/game-score/{gameId}/all", ScoreController.HandleGetGameScores).RequireAuthorization("client");
app.MapDelete("api/v1/game-score/{scoreId}", ScoreController.HandleDeleteGameScore).RequireAuthorization("super-admin");

app.Run();