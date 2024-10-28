using Chess.Core.Constants;
using Chess.Core.Extensions;
using Chess.Core.Manage;
using Chess.Core.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace Chess.Core.SignalR;

public class GameHub(ILogger<HubBase> logger, SessionManager sessionManager, GameManager gameManager) : HubBase(logger)
{
    [Obsolete("Recommended to use endpoint: 'game/init'")]
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
        var game = await gameManager.GetAsync(player.GameId.GetValueOrDefault()) ??
                   await CreateGameSessionAsync(player);
        game = await gameManager.AddPlayerAsync(game.Id, player);
        
        NullException.ThrowIfNull(game);
        var session = sessionManager.GetSession(game.Id);

        if (session == null)
        {
            sessionManager.CreateSession(game.Id);
            foreach (var pl in game.Players)
            {
                Logger.LogInformation("Player:{@player} added to Game: {@game}", pl, game);
                sessionManager.AddPlayerToGame(game.Id, pl.UserId, Context.ConnectionId);
            }
        }

        await Groups.AddToGroupAsync(ConnectionId, game.Id.ToString());

        sessionManager.AddPlayerToGame(game.Id, player.UserId, Context.ConnectionId);
        
        await Clients.Group(game.Id.ToString()).SendAsync(HubMethods.Joined, game);
    }

    [HubMethodName(HubMethods.LeaveGame)]
    public async Task LeaveGameAsync(Player player)
    {
        NullException.ThrowIfNull(player.GameId);
        var session = sessionManager.GetSession(player.GameId.GetValueOrDefault());
        if (session is null)
        {
            logger.LogWarning("Session is not found for Game: {game}", player.GameId);
            return;
        }


        sessionManager.RemovePlayerFromGame(session.GameId, player.UserId);
        if (session.ConnectionId(player.UserId) is not null && player.GameId.ToString() is not null)
        {
            await Groups.RemoveFromGroupAsync(session.ConnectionId(player.UserId)!, player.GameId.ToString()!);
            await Clients.Group($"{player.GameId}").SendAsync(HubMethods.Left, player.UserId);
        }

        logger.LogInformation("Player: {player} left Game: {game}", player.UserId, player.UserId);
    }

    [HubMethodName(HubMethods.Move)]
    public async ValueTask MoveAsync(MoveRequest request)
    {
        var session = sessionManager.GetSession(request.Player.GameId.GetValueOrDefault());
        ArgumentNullException.ThrowIfNull(request.Player.GameId);
        if (session is null)
        {
            logger.LogInformation("Session not found for Game: {game}", request.Player.GameId);
            return;
        }

        if (session.IsPlayerInGame(request.Player.UserId))
        {
            var game = await gameManager.MoveAsync(request);
            await Clients.Group($"{request.Player.GameId}").SendAsync(HubMethods.Moved, game);
        }
    }

    private async ValueTask<GameModel> CreateGameSessionAsync(Player player)
    {
        var game = await gameManager.InitGameAsync(new InitGame(new List<Player> { player }));
        NullException.ThrowIfNull(game);
        sessionManager.CreateSession(game.Id);
        Logger.LogInformation("Game initialized: '{@game}'", game.Id);
        foreach (var pl in game.Players)
        {
            Logger.LogInformation("Player:{@player} added to Game: {@game}", pl, game);
            sessionManager.AddPlayerToGame(game.Id, pl.UserId, Context.ConnectionId);
        }

        await Groups.AddToGroupAsync(ConnectionId, game.Id.ToString());
        return game;
    }
}