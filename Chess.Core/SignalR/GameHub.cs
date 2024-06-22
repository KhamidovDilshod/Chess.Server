using Chess.Core.Constants;
using Chess.Core.Manage;
using Chess.Core.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace Chess.Core.SignalR;

public class GameHub(ILogger<HubBase> logger, SessionManager sessionManager, GameManager gameManager)
    : HubBase(logger)
{
    [HubMethodName(HubMethods.CreateGame)]
    public async ValueTask CreateGameAsync(InitGame init)
    {
        var game = await gameManager.InitGameAsync(init);
        sessionManager.CreateSession(game.Id);
        Logger.LogInformation("Game initialized: '{@game}'", game.Id);
        foreach (var player in game.Players)
        {
            Logger.LogInformation("Player:{@player} added to Game: {@game}", player, game);
            sessionManager.AddPlayerToGame(game.Id, player.UserId, Context.ConnectionId);
        }

        await Groups.AddToGroupAsync(ConnectionId, game.Id.ToString());
        await Clients
            .Group(Context.ConnectionId)
            .SendAsync(HubMethods.CreateGame, game);
    }

    [HubMethodName(HubMethods.JoinGame)]
    public async ValueTask JoinGameAsync(Player player)
    {
        var game = await gameManager.GetAsync(player.GameId.GetValueOrDefault());
        if (game is null)
        {
            await CreateGameAsync(new InitGame(new List<Player> { player }));
            return;
        }

        var session = sessionManager.GetSession(game.Id);

        if (session == null)
        {
            sessionManager.CreateSession(game.Id);
            await Groups.AddToGroupAsync(ConnectionId, game.Id.ToString());
            foreach (var pl in game.Players)
            {
                Logger.LogInformation("Player:{@player} added to Game: {@game}", pl, game);
                sessionManager.AddPlayerToGame(game.Id, pl.UserId, Context.ConnectionId);
            }
        }

        sessionManager.AddPlayerToGame(game.Id, player.UserId, Context.ConnectionId);
        game = await gameManager.AddPlayerAsync(game.Id, player);
        await Clients.Group(game!.Id.ToString())
            .SendAsync(HubMethods.JoinGame, game);
    }

    public async ValueTask LeaveGameAsync()
    {
    }
}