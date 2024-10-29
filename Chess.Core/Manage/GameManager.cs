using System.Text.Json;
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
    public async ValueTask<GameModel?> InitGameAsync(InitGame init)
    {
        var (game, players) = Game.Init(init);
        var board = new ChessBoard().ChessBoardView;

        game.Id = Guid.NewGuid();
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
        return await GetAsync(game.Id);
    }

    public async ValueTask<GameModel?> GetAsync(Guid gameId)
    {
        return (await Get<Game, Guid>(gameId)).ToModel(await GetAll<GamePlayer>(p => p.GameId == gameId));
    }


    public async ValueTask<GameModel?> AddPlayerAsync(Guid gameId, Player player)
    {
        var game = await Get(gameId);
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

    public async ValueTask<char[][]> MoveAsync(MoveRequest request)
    {
        var gameId = request.Player.GameId.GetValueOrDefault();
        var gameBoard = await GetBoardByGameId(gameId);
        var game = await Get(gameId);

        if (game is null || gameBoard.state is null)
        {
            throw new Exception("Board not found");
        }

        game.Players = await GetAll<GamePlayer>(g => g.GameId == game.Id);

        string notation = $"prevX:{request.PrevX}-prevY:{request.PrevY},newX:{request.NewX}-newY{request.NewY}";
        //TODO save Initialized board in cache instead of initializing it from state
        //Create singleton BoardCacheManager Class Map<gameId,ChessBoard> like map
        var board = new ChessBoard(gameBoard.state, request.Player.Color);

        board.Move(request.PrevX, request.PrevY, request.NewX, request.NewY);
        var move = game.AddMove(
            new AddMove(
                request.Player.GameId.GetValueOrDefault(),
                request.Player.UserId,
                game.LastMove()?.Number ?? 1,
                notation
            ));
        await Add(move);
        //TODO replace every time board updating on move to save on cache and persist to db in every (x) time(Maybe bad option!)
        await EnsureBoardUpdated(gameId, gameBoard.id, board);
        return board.ChessBoardView;
    }

    private async Task<Game?> Get(Guid id)
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