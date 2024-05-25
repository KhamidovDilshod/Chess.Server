namespace Chess.Core.Persistence.Entities;

public class Game : Entity
{
    public long WhitePlayerId { get; set; }
    public long BlackPlayerId { get; set; }

    public User? WhitePlayer { get; set; }
    public User? BlackPlayer { get; set; }
}