using Chess.Core.Logic;
using Chess.Core.Models;

namespace Chess.Core.Persistence.Entities;

[BsonCollection("game")]
public class Game : Entity
{
    public List<GamePlayer> Players { get; set; } = new();
    public Board? Board { get; set; }

    public static (Game game, IEnumerable<GamePlayer> players) Init(InitGame init)
    {
        //TODO remove
        if (init.Players == null || !init.Players.Any())
            throw new ArgumentException("At least one player must be provided", nameof(init));

        var game = new Game();
        var players = init.Players.Select(player => GamePlayer.Create(player, game.Id));
        return (game, players);
    }

    public bool TryAddPlayer(Player player, out GamePlayer? gamePlayer)
    {
        gamePlayer = Players.FirstOrDefault(p => p.UserId == player.UserId);
        if (gamePlayer != null) return false; // Player already in game

        var mainPlayers = Players.Where(p => p.Color != Color.Null).ToList();
        if (mainPlayers.Count >= 2)
        {
            gamePlayer = GamePlayer.Create(player with { Color = Color.Null });
        }
        else
        {
            var nextColor = mainPlayers.Count switch
            {
                0 => Color.White,
                _ => mainPlayers[0].Color == Color.Black ? Color.White : Color.Black
            };
            gamePlayer = GamePlayer.Create(player with { Color = nextColor });
        }

        Players.Add(gamePlayer);
        return true;
    }

    public bool CanMove(AddMove move)
    {
        var player = Players.FirstOrDefault(p => p.UserId == move.UserId);
        if (player == null) return false;

        return player.Color != Color.Null;
    }
    
}