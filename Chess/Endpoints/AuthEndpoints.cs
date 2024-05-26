using Chess.Core.Manage;
using Chess.Core.Models;

namespace Chess.Endpoints;

public static class AuthEndpoints
{
    public static void AddAuthEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("users/{id:long}", async (UserManager manager, long id) =>
        {
            var user = await manager.Get(id);
            return user is null ? Results.NotFound() : Results.Ok(user);
        });

        app.MapGet("users", async (UserManager manager) => Results.Ok(await manager.GetAll()));

        app.MapPost("users", async (UserManager manager, UserCreate userCreate) =>
        {
            var user = await manager.CreateUserAsync(userCreate);
            return Results.Ok(user);
        });
    }
}