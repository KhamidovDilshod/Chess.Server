using System.Linq.Expressions;
using System.Text.Json;
using Chess.Core.Logic;
using Chess.Core.Models;
using Chess.Core.Persistence;
using Chess.Core.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;

namespace Chess.Core.Manage;

public class GameManager(IMongoDatabase db) : BaseManager<Game, GameModel, Guid>(db)
{
    protected override Expression<Func<Game, GameModel>> EntityToModel => e
        => new GameModel(
            e.Id,
            e.Date,
            e.Players.Select(p => new Player(p.UserId, p.GameId, p.Color)).ToList()
        );

    public async ValueTask<GameModel> InitGameAsync(InitGame init)
    {
        var game = Game.Init(init);
        var board = new ChessBoard().ChessBoardView;
        game.Board = new Board
        {
            StateJson = JsonSerializer.Serialize(board)
        };
        return await Add(game);
    }

    public async ValueTask<GameModel?> AddPlayerAsync(Guid gameId, Player player)
    {
        var game = await Database.Games.FirstOrDefaultAsync(g => g.Id == gameId);
        if (game is null) return null;
        game.AssignPlayer(player);
        await Database.SaveChangesAsync();
        return ToGameModel(game);
    }

    public async Task<GameModel?> GetById(Guid id)
    {
        // Fetch the game entity by ID
        var game = await Database.Games
            .FirstOrDefaultAsync(g => g.Id == id);

        // Explicitly load related entities if they are not automatically loaded
        if (game == null) return ToGameModel(game);
        {
            await Database.Entry(game)
                .Collection(g => g.Players)
                .LoadAsync();
        }

        // Convert the entity to the model
        return ToGameModel(game);
    }

    public async ValueTask<BoardModel?> GetBoardByGameId(Guid gameId)
    {
        var board = await Database.Boards.Where(b => b.GameId == gameId)
            .FirstOrDefaultAsync();
        return ToBoardModel(board);
    }

    private GameModel? ToGameModel(Game? game)
    {
        if (game is null) return null;
        return new GameModel(
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