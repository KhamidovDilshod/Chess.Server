using System.Linq.Expressions;
using Chess.Core.Models;
using Chess.Core.Persistence;
using Chess.Core.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;

namespace Chess.Core.Manage;

public class MoveManager(IMongoDatabase db) : BaseManager<Move, MoveModel, Guid>(db)
{
    protected override Expression<Func<Move, MoveModel>> EntityToModel =>
        e => new MoveModel(e.Id, e.GameId, e.Number, e.Notation);

    public async ValueTask<MoveModel?> AddMoveToGame(AddMove move)
    {
        var game = Database.Games
            .Include(g => g.Players)
            .Include(g => g.Moves)
            .FirstOrDefault(g => g.Id == move.GameId);

        if (game is null) return null;
        var entity = game.AddMove(move);
        await Database.SaveChangesAsync();
        return EntityToModel.Compile().Invoke(entity);
    }
}