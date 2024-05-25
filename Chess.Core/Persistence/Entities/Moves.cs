namespace Chess.Core.Persistence.Entities;

public class Moves
{
    public Guid GameId { get; set; }
    public int Number { get; set; }
    public required string Notation { get; set; }
    
    public Game? Game { get; set; }
}