using Chess.Core.Manage;
using Chess.Core.Models;

namespace Chess.Endpoints;

public static class GameEndpoint
{
    public static void AddGameEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("game/{gameId}", async (GameManager manager, Guid gameId) =>
        {
            var result = await manager.GetAsync(gameId);
            return result is null ? Results.NotFound() : Results.Ok(result);
        });

        app.MapPost("game/init", async (GameManager manager, InitGame init) =>
        {
            var result = await manager.InitGameAsync(init);
            return Results.Ok(result);
        });

        app.MapPost("game/{gameId}/player", async (GameManager manager, Guid gameId, Player player) =>
        {
            var result = await manager.AddPlayerAsync(gameId, player);
            return result is null ? Results.NotFound() : Results.Ok(result);
        });
        app.MapGet("game/{gameId}/board", async (GameManager manager, Guid gameId) =>
        {
            var result = await manager.GetBoardByGameId(gameId);
            return result is null ? Results.NotFound() : Results.Ok(result);
        });
    }
}