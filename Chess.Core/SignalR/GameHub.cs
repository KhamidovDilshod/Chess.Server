using Chess.Core.Constants;
using Chess.Core.Manage;
using Chess.Core.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace Chess.Core.SignalR;

public class GameHub(ILogger<HubBase> logger, SessionManager sessionManager, GameManager gameManager)
    : HubBase(logger)
{
    [Obsolete]
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

    [HubMethodName(HubMethods.LeaveGame)]
    public void LeaveGameAsync(Player player)
    {
        var session = sessionManager.GetSession(player.GameId.GetValueOrDefault());
        if (session is null)
        {
            logger.LogWarning("Session is not found for Game: {game}", player.GameId);
            return;
        }

        sessionManager.RemovePlayerFromGame(session.GameId, player.UserId);
        if (session.ConnectionId(player.UserId) is not null && player.GameId.ToString() is not null)
        {
            Groups.RemoveFromGroupAsync(session.ConnectionId(player.UserId)!, player.GameId.ToString()!);
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
            Clients.Group($"{request.Player.GameId}")?.SendAsync(HubMethods.Move, game);
        }
    }

    private async ValueTask<Game> CreateGameSessionAsync(Player player)
    {
        var game = await gameManager.InitGameAsync(new InitGame { Players = new List<Player> { player } });
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
public record MoveRequest(Player Player, int PrevX, int PrevY, int NewX, int NewY);