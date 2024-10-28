using Chess.Core.Logic;
using Chess.Core.Models;

namespace Chess.Core.Persistence.Entities;

[BsonCollection("game")]
public class Game : Entity
{
    public List<GamePlayer> Players { get; set; } = new();
    public List<Move> Moves { get; init; } = new();
    public Board? Board { get; set; }

    public static (Game game, IEnumerable<GamePlayer> players) Init(InitGame init)
    {
        var gameId = Guid.NewGuid();
        var game = new Game
        {
            Id = gameId
        };

        return (game, init.Players?.Select(player => GamePlayer.Create(player, gameId)) ??
                      Enumerable.Empty<GamePlayer>());
    }

    public bool TryAddPlayer(Player player, out GamePlayer? gamePlayer)
    {
        gamePlayer = Players.FirstOrDefault(p => p.UserId == player.UserId);
        if (gamePlayer != null) return false;

        var mainPlayers = Players.Where(p => p.Color != Color.Null).ToList();
        if (!mainPlayers.Any())
        {
            gamePlayer = GamePlayer.Create(player);
            Players.Add(gamePlayer);
            return true;
        }

        if (mainPlayers.Count >= 2)
        {
            player = player with { Color = Color.Null };
        }
        else
        {
            player = player with { Color = Players.First().Color == Color.Black ? Color.White : Color.Black };
        }

        gamePlayer = GamePlayer.Create(player);
        Players.Add(gamePlayer);
        return true;
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

    public Move? LastMove() => Moves.MaxBy(move => move.Date);
}