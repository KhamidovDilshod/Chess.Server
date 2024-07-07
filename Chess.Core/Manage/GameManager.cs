using System.Linq.Expressions;
using System.Text.Json;
using Chess.Core.Logic;
using Chess.Core.Models;
using Chess.Core.Persistence;
using Chess.Core.Persistence.Entities;
using Chess.Core.SignalR;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using Game = Chess.Core.Persistence.Entities.Game;

namespace Chess.Core.Manage;

public class GameManager(IMongoDatabase db) : BaseManager<Game, Models.Game, Guid>(db)
{
    protected override Expression<Func<Game, Models.Game>> EntityToModel => e
        => new Models.Game(
            e.Id,
            e.Date,
            e.Players.Select(p => new Player(p.UserId, p.GameId, p.Color)).ToList()
        );

    public async ValueTask<Models.Game> InitGameAsync(InitGame init)
    {
        var game = Game.Init(init);
        var board = new ChessBoard().ChessBoardView;
        game.Board = new Board
        {
            StateJson = JsonSerializer.Serialize(board)
        };
        return await Add(game);
    }

    public async ValueTask<Models.Game?> GetAsync(Guid id)
    {
        var game = await Database.Games
            .Where(g => g.Id == id)
            .FirstOrDefaultAsync();

        if (game is not null)
        {
            await Database.Entry(game)
                .Collection(g => g.Players)
                .LoadAsync();
        }

        return ToGameModel(game);
    }


    public async ValueTask<Models.Game?> AddPlayerAsync(Guid gameId, Player player, Game? game = null)
    {
        game ??= await Get(gameId);
        game!.AssignPlayer(player);
        await Database.SaveChangesAsync();
        return ToGameModel(game);
    }

    public new async Task<Game?> Get(Guid id)
    {
        var game = await Database.Games
            .FirstOrDefaultAsync(g => g.Id == id);

        if (game == null) return game;
        {
            await Database.Entry(game)
                .Collection(g => g.Players)
                .LoadAsync();
        }
        return game;
    }

    public async Task<Models.Game?> GetById(Guid id)
    {
        var game = await Database.Games
            .FirstOrDefaultAsync(g => g.Id == id);

        if (game == null) return ToGameModel(game);
        {
            await Database.Entry(game)
                .Collection(g => g.Players)
                .LoadAsync();
        }
        return ToGameModel(game);
    }

    public async ValueTask<string[][]> GetBoardByGameId(Guid gameId)
    {
        try
        {
            var board = await Database.Boards.Where(b => b.GameId == gameId)
                .FirstOrDefaultAsync();
            
            return ToBoardModel(board)?.State ?? new string[][] { };
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public async ValueTask<string?[][]> MoveAsync(MoveRequest request)
    {
            var gameId = request.Player.GameId.GetValueOrDefault();
            var game = await Get(gameId);
            var gameBoard = await GetBoardByGameId(gameId);

            if (game is null || gameBoard is null)
            {
                throw new Exception("Board not found");
            }

            var boardState = JsonSerializer.Deserialize<char[][]>(game.Board.StateJson);

            if (boardState is null) throw new Exception("Can't deserialize board from db");

            var board = new ChessBoard(boardState);
            board.Move(request.PrevX, request.PrevY, request.NewX, request.NewY);
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
            : new BoardModel(JsonSerializer.Deserialize<string[][]>(board.StateJson, JsonSerializerOptions) ??
                             Array.Empty<string[]>());
    }
}