
namespace Chess.Core.Persistence.Entities;

[BsonCollection("board")]
public class Board : Entity
{
    public Guid GameId { get; set; }
    public string StateJson { get; set; } = "{}";
    public Game? Game { get; set; }
}