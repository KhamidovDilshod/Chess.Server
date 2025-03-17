using System.Text.Json;
using Chess.Core.Exceptions;
using Chess.Core.Extensions;
using Chess.Core.Logic;
using Chess.Core.Models;
using Chess.Core.Persistence;
using Chess.Core.Persistence.Entities;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Chess.Core.Manage;

public class GameManager(IOptions<MongoOptions> options) : MongoDb(options.Value), IManager
{
    public async ValueTask<GameModel> InitGameAsync(InitGame init)
    {
        var (game, players) = Game.Init(init);
        var board = new ChessBoard().ChessBoardView;
        var gameBoard = new Board
        {
            Id = game.Id,
            GameId = game.Id,
            StateJson = JsonSerializer.Serialize(board)
        };

        var tasks = players.Select(Add).ToArray();
        await Task.WhenAll(tasks);

        await Add(game);
        await Add(gameBoard);
        return await GetAsync(game.Id)?? throw new Exception("Null game on initialization");
    }

    public async ValueTask<GameModel?> GetAsync(Guid gameId)
    {
        return (await Get<Game, Guid>(gameId)).ToModel(await GetAll<GamePlayer>(p => p.GameId == gameId));
    }


    public async ValueTask<GameModel?> AddPlayerAsync(Guid gameId, Player player)
    {
        var game = await GetGame(gameId);
        if (game is null) return null;
        game.Players = await GetAll<GamePlayer>(g => g.GameId == game.Id);
        if (game.TryAddPlayer(player, out var gamePlayer))
        {
            await Add(gamePlayer);
        }

        return await GetAsync(gameId);
    }

    public async ValueTask<(Guid id, char[][] state)> GetBoardByGameId(Guid gameId)
    {
        var board = await Set<Board>().Find(board => board.GameId == gameId).FirstOrDefaultAsync();
        try
        {
            return (board.Id, board.ToModel()?.State ?? new char[][] { });
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return (board.Id, new char[][] { });
        }
    }

    public async Task<List<MoveModel>> GetMovesByGameId(Guid gameId)
    {
        return (await GetAll<Move>(move => move.GameId == gameId)).Select(m => m.ToModel()).ToList();
    }

    public async ValueTask<ReadOnlyMemory<char>> MoveAsync(MoveRequest request)
    {
        var gameId = request.Player.GameId.GetValueOrDefault();
        var (boardId, boardState) = await GetBoardByGameId(gameId);

        var game = await GetGame(gameId);
        if (game is null || boardState is null)
        {
            throw new GameException(Codes.BoardNotFound);
        }

        game.Players = await GetAll<GamePlayer>(g => g.GameId == game.Id);

        string notation = $"prevX:{request.PrevX}-prevY:{request.PrevY},newX:{request.NewX}-newY:{request.NewY}";

        var board = new ChessBoard(boardState, request.Player.Color);
        board.Move(request.PrevX, request.PrevY, request.NewX, request.NewY);

        //TODO move number??
        var addMove = new AddMove(
            gameId,
            request.Player.UserId,
            1,
            notation
        );

        if (game.CanMove(addMove))
        {
            var entity = Move.Create(addMove);

            await Add(entity);
        }

        await EnsureBoardUpdated(gameId, boardId, board);
        var flatBoard = FlattenChessBoard(board.ChessBoardView);

        return new ReadOnlyMemory<char>(flatBoard);
    }

    private static char[] FlattenChessBoard(char[][] chessBoard)
    {
        int rows = chessBoard.Length;
        int cols = chessBoard[0].Length;
        var flatBoard = new char[rows * cols];

        for (int i = 0; i < rows; i++)
        {
            Array.Copy(chessBoard[i], 0, flatBoard, i * cols, cols);
        }

        return flatBoard;
    }


    private async Task<Game?> GetGame(Guid id)
    {
        var game = await Get<Game, Guid>(id);
        return game;
    }

    private Task EnsureBoardUpdated(Guid gameId, Guid boardId, ChessBoard chessBoard)
    {
        var newBoard = new Board
        {
            Id = boardId,
            GameId = gameId,
            StateJson = JsonSerializer.Serialize(chessBoard.ChessBoardView)
        };
        return Update(newBoard);
    }
}