using System.Text.Json;
using Chess.Core.Logic;
using Chess.Core.Models;
using Chess.Core.Persistence;
using Chess.Core.Persistence.Entities;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Game = Chess.Core.Persistence.Entities.Game;

namespace Chess.Core.Manage;

public class GameManager(IOptions<MongoOptions> options) : BaseManager(options.Value), IManager
{
    public async ValueTask<Models.Game?> InitGameAsync(InitGame init)
    {
        var game = Game.Init(init);
        var board = new ChessBoard().ChessBoardView;
        var gameBoard = new Board
        {
            Id = Guid.NewGuid(),
            GameId = game.Id,
            StateJson = JsonSerializer.Serialize(board)
        };
        await Add(game);
        await Add(gameBoard);
        return ToGameModel(await Get<Game, Guid>(game.Id));
    }

    public async ValueTask<Models.Game?> GetAsync(Guid gameId)
    {
        return ToGameModel(await Get<Game, Guid>(gameId));
    }


    public async ValueTask<Models.Game?> AddPlayerAsync(Guid gameId, Player player, Game? game = null)
    {
        game ??= await Get(gameId);
        game!.AssignPlayer(player);

        foreach (var gamePlayer in game.Players)
        {
            await Add(gamePlayer);
        }

        return await GetAsync(game.Id);
    }

    public async Task<Game?> Get(Guid id)
    {
        var game = await Get<Game, Guid>(id);

        return game;
    }

    public async ValueTask<char[][]> GetBoardByGameId(Guid gameId)
    {
        try
        {
            var board = await Set<Board>().Find(board => board.GameId == gameId).FirstOrDefaultAsync();
            return ToBoardModel(board)?.State ?? new char[][] { };
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return new char[][] { };
        }
    }

    public async ValueTask<char[][]> MoveAsync(MoveRequest request)
    {
        var gameId = request.Player.GameId.GetValueOrDefault();
        var gameBoard = await GetBoardByGameId(gameId);
        var game = await Get(gameId);

        if (game is null || gameBoard is null)
        {
            throw new Exception("Board not found");
        }

        string notation = $"prevX:{request.PrevX}-prevY:{request.PrevY},newX:{request.NewX}-newY{request.NewY}";


        if (gameBoard is null) throw new Exception("Can't deserialize board from db");

        var board = new ChessBoard(gameBoard);
        
        board.Move(request.PrevX, request.PrevY, request.NewX, request.NewY);
        
        game.AddMove(
            new AddMove(
                request.Player.GameId.GetValueOrDefault(),
                request.Player.UserId,
                game.LastMove()?.Number ?? 0 + 1,
                notation
            ));

        return board.ChessBoardView;
    }

    private Models.Game? ToGameModel(Game? game)
    {
        if (game is null) return null;
        return new Models.Game(
            game.Id,
            game.Date,
            game.Players.Select(p => new Player(p.UserId, p.GameId, p.Color)).ToList());
    }

    private BoardModel? ToBoardModel(Board? board)
    {
        return string.IsNullOrEmpty(board?.StateJson)
            ? null
            : new BoardModel(JsonSerializer.Deserialize<char[][]>(board.StateJson, JsonSerializerOptions) ??
                             Array.Empty<char[]>());
    }
}