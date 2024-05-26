using Chess.Core.Logic;
using Chess.Core.Models;

namespace Chess.Core.Persistence.Entities;

public class Game : Entity
{
    public Guid BoardId { get; set; }
    public List<GamePlayer> Players { get; init; } = [];
    public List<Move> Moves { get; init; } = [];
    public Board? Board { get; set; }

    public static Game Init(InitGame init)
    {
        var game = new Game();
        if (init.Players is not null)
        {
            game.Players.AddRange(init.Players.Select(GamePlayer.Create));
        }

        return game;
    }

    public void AssignPlayer(Player player)
    {
        if (Players.Any(p => p.Color == player.Color))
        {
            throw new Exception($"Player with color: {player.Color} already exist");
        }

        Players.Add(GamePlayer.Create(player));
    }

    public Move AddMove(AddMove move)
    {
        var player = Players.FirstOrDefault(p => p.UserId == move.UserId);
        if (player is null || player.Color == Color.Null)
        {
            throw new Exception("Player doesn't have permission to move");
        }

        var entity = Move.Create(move);
        Moves.Add(entity);
        return entity;
    }
}