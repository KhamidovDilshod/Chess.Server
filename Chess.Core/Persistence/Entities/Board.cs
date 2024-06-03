
namespace Chess.Core.Persistence.Entities;

public class Board : Entity
{
    public Guid GameId { get; set; }
    public string StateJson { get; set; } = "{}";
    public Game? Game { get; set; }
}