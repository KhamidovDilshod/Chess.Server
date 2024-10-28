using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Chess.Core.Persistence.Entities;

[BsonCollection("gameState")]
public class GameState : Entity
{
    [BsonRepresentation(BsonType.String)]
    public Guid GameId { get; set; }
    public GameStatus Status { get; set; }
    
    public Game? Game { get; set; }
}