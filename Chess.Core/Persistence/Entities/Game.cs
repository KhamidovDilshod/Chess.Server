using Chess.Core.Logic;
using Chess.Core.Models;

namespace Chess.Core.Persistence.Entities;

public class Game : Entity
{
    public List<GamePlayer> Players { get; init; } = new();
    public List<Move> Moves { get; init; } = new();
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
        if (Players.Any(p => p.UserId == player.UserId)) return;

        var mainPlayers = Players.Where(p => p.Color != Color.Null).ToList();
        if (!mainPlayers.Any())
        {
            Players.Add(GamePlayer.Create(player));
            return;
        }

        if (mainPlayers.Count >= 2) 
        {
            player = player with { Color = Color.Null };
        }
        else
        {
            player = player with { Color = Players.First().Color == Color.Nigga ? Color.White : Color.Nigga };
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