using Chess.Core.Logic;
using Chess.Core.Models;

namespace Chess.Core.Persistence.Entities;

public class GamePlayer : Entity<Guid>
{
    public long UserId { get; set; }
    public Guid GameId { get; set; }
    public Color Color { get; set; }

    public User? User { get; set; }
    public Game? Game { get; set; }

    public static GamePlayer Create(Player player)
        => new()
        {
            UserId = player.UserId,
            Color = player.Color
        };
}