using Chess.Core.Logic;

namespace Chess.Core.Models;

#region Game

public record InitGame(List<Player>? Players = null);

public record GameModel(Guid Id, DateTime Date, List<Player> Players);

public record Player(Guid UserId, Guid? GameId, Color Color);

#endregion

#region Move

public record MoveModel(Guid Id, Guid GameId, Guid UserId, int Number, string Notation);

public record AddMove(Guid GameId, Guid UserId, int Number, string Notation);

public record MoveRequest(Player Player, int PrevX, int PrevY, int NewX, int NewY);

#endregion

#region Board

public record BoardModel(char[][] State);

#endregion

#region User

public record UserModel(Guid Id, string Username, string Email, DateTime Date);

public record UserCreate(string Username, string Email, string LogoUrl);

#endregion