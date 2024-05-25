namespace Chess.Core.Persistence.Entities;

public class User : Entity<long>
{
    public required string UserName { get; set; }
    public required string Email { get; set; }
    public string LogoUrl { get; set; } = string.Empty;
}

public enum GameStatus
{
    Ongoing,
    WhiteWon,
    BlackWon,
    Draw
}