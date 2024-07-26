namespace Chess.Core.Persistence.Entities;

[BsonCollection("gameState")]
public class GameState : Entity
{
    public Guid GameId { get; set; }
    public GameStatus Status { get; set; }
    
    public Game? Game { get; set; }
}