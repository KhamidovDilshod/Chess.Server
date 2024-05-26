using System.ComponentModel.DataAnnotations;

namespace Chess.Core.Persistence.Entities;

public class User : Entity<long>
{
    [MaxLength(60)] public required string Username { get; set; }
    [MaxLength(70)] public required string Email { get; set; }
    [MaxLength(500)] public string LogoUrl { get; set; } = string.Empty;

    public static User Create(string username, string email, string LogoUrl)
    {
        return new User
        {
            Username = username,
            Email = email,
            LogoUrl = LogoUrl
        };
    }
}

public enum GameStatus
{
    Ongoing,
    WhiteWon,
    BlackWon,
    Draw
}