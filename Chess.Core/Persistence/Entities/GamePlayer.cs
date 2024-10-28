using Chess.Core.Logic;
using Chess.Core.Models;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Chess.Core.Persistence.Entities;

[BsonCollection("gamePlayer")]
public class GamePlayer : Entity
{
    [BsonRepresentation(BsonType.String)] public Guid UserId { get; set; }
    [BsonRepresentation(BsonType.String)] public Guid GameId { get; set; }
    public Color Color { get; set; }

    public User? User { get; set; }
    public Game? Game { get; set; }

    public static GamePlayer Create(Player player) => Create(player, player.GameId.GetValueOrDefault());

    public static GamePlayer Create(Player player, Guid gameId)
        => new()
        {
            Id = Guid.NewGuid(),
            UserId = player.UserId,
            Color = player.Color,
            GameId = gameId
        };

    public Player ToModel() => new(UserId, GameId, Color);
}