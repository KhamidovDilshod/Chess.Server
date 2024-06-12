using Chess.Core.Logic;

namespace Chess.Core.Models;

#region Game Models

public record InitGame(List<Player>? Players = null);

public record GameModel(Guid Id, DateTime Date, List<Player> Players);

public record Player(Guid UserId, Guid? GameId, Color Color);

#endregion

#region Move Models

public record MoveModel(Guid Id, Guid GameId, int Number, string Notation);

public record AddMove(Guid GameId, Guid UserId, int Number, string Notation);

#endregion

#region Board

public record BoardModel(string[][] State);

#endregion

#region User

public record UserModel(Guid Id, string Username, string Email, DateTime Date);

public record UserCreate(string Username, string Email, string LogoUrl);

#endregion