using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using Chess.Core.Models;
using Chess.Core.Persistence.Entities;

namespace Chess.Core.Extensions;

public static class ModelExtensions
{
    private static readonly JsonSerializerOptions JsonSerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public static GameModel? ToModel(this Game? game, IEnumerable<GamePlayer>? players = null)
    {
        if (game is null) return null;
        return new GameModel(
            game.Id,
            game.Date,
            players?.Select(p => new Player(p.UserId, p.GameId, p.Color)).ToList() ?? new List<Player>());
    }

    public static BoardModel? ToModel(this Board? board)
    {
        return string.IsNullOrEmpty(board?.StateJson)
            ? null
            : new BoardModel(JsonSerializer.Deserialize<char[][]>(board.StateJson, JsonSerializerOptions) ??
                             Array.Empty<char[]>());
    }

    public static MoveModel ToModel(this Move move)
    {
        return new MoveModel(move.Id, move.GameId, move.UserId, move.Number, move.Notation);
    }
}

public static class NullException
{
    public static void ThrowIfNull<T>([NotNull] T? value) where T : class
    {
        if (value == null) throw new ArgumentNullException();
    }

    public static void ThrowIfNull<T>([NotNull] T? value) where T : struct
    {
        if (value == null) throw new ArgumentNullException();
    }
}