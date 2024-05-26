using System.ComponentModel.DataAnnotations;
using Chess.Core.Models;

namespace Chess.Core.Persistence.Entities;

public class Move : Entity
{
    public Guid GameId { get; set; }
    public long UserId { get; set; }
    public int Number { get; set; }
    [StringLength(30)] public required string Notation { get; set; }

    public Game? Game { get; set; }
    public User? User { get; set; }

    public static Move Create(AddMove move)
    {
        return new Move
        {
            GameId = move.GameId,
            UserId = move.UserId,
            Number = move.Number,
            Notation = move.Notation
        };
    }
}