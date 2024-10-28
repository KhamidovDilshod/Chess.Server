using System.ComponentModel.DataAnnotations;

namespace Chess.Core.Persistence.Entities;

[BsonCollection("user")]
public class User : Entity
{
    [MaxLength(60)] public required string Username { get; set; }
    [MaxLength(70)] public required string Email { get; set; }
    [MaxLength(500)] public string LogoUrl { get; set; } = string.Empty;

    public static User Create(string username, string email, string logoUrl)
    {
        return new User
        {
            Id = Guid.NewGuid(),
            Username = username,
            Email = email,
            LogoUrl = logoUrl
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