using Chess.Core.Logic;
using Chess.Core.Persistence.Entities;

namespace Chess.Core.Models;

#region Game Models

public record InitGame(List<Player>? Players = null);

public record GameModel(Guid Id, DateTime Date, List<Player> Players, BoardModel Board);

public record Player(long UserId, Guid? GameId, Color Color);

#endregion

#region Move Models

public record MoveModel(Guid Id, Guid GameId, int Number, string Notation);

public record AddMove(Guid GameId, long UserId, int Number, string Notation);

#endregion

#region Board

public record BoardModel(string[][] State);

#endregion

#region User

public record UserModel(long Id, string Username, string Email, DateTime Date);

public record UserCreate(string Username, string Email, string LogoUrl);

#endregion