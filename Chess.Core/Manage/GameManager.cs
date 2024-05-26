using System.Linq.Expressions;
using System.Text.Json;
using Chess.Core.Logic;
using Chess.Core.Models;
using Chess.Core.Persistence;
using Chess.Core.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;

namespace Chess.Core.Manage;

public class GameManager(IMongoDatabase db) : BaseManager<Game, GameModel,Guid>(db)
{
    protected override Expression<Func<Game, GameModel>> EntityToModel => e
        => new GameModel(
            e.Id,
            e.Date,
            e.Players.Select(p => new Player(p.UserId, p.GameId, p.Color)).ToList(),
            new BoardModel(e.Board.State.Deserialize<string[][]>(JsonSerializerOptions))
        );

    public async ValueTask<GameModel> InitGameAsync(InitGame init)
    {
        var game = Game.Init(init);
        var board = new ChessBoard().ChessBoardView;
        game.Board = new Board
        {
            State = JsonSerializer.SerializeToDocument(board)
        };
        return await Add(game);
    }

    public async ValueTask<GameModel?> AddPlayerAsync(Guid gameId, Player player)
    {
        var game = await Database.Games.FirstOrDefaultAsync(g => g.Id == gameId);
        if (game is null) return null;
        game.AssignPlayer(player);
        await Database.SaveChangesAsync();
        return EntityToModel.Compile().Invoke(game);
    }
}