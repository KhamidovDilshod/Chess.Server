using System.ComponentModel.DataAnnotations;
using Chess.Core.Models;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Chess.Core.Persistence.Entities;

[BsonCollection("move")]
public class Move : Entity
{
    [BsonRepresentation(BsonType.String)] public Guid GameId { get; set; }
    [BsonRepresentation(BsonType.String)] public Guid UserId { get; set; }
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
            Notation = move.Notation,
            Id = Guid.NewGuid()
        };
    }
}