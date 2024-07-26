using Chess.Core.Manage;
using Chess.Core.Models;
using Chess.Core.Persistence.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Chess.Endpoints;

public static class AuthEndpoints
{
    public static void AddAuthEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("users/{id}", async (UserManager manager, Guid id) =>
        {
            var user = await manager.Get<User,Guid>(id);
            return user is null ? Results.NotFound() : Results.Ok(user);
        });

        app.MapGet("users", async (UserManager manager) => Results.Ok(await manager.GetAll<User>()));

        app.MapPost("users", async (UserManager manager, UserCreate userCreate) =>
            Results.Ok(await manager.GetOrCreateUserAsync(userCreate)));
    }
}