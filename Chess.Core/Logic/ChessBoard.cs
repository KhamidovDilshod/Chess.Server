using Chess.Core.Logic.Pieces;

namespace Chess.Core.Logic;

public class ChessBoard
{
    private readonly Piece?[][] _chessBoard;
    private Color _playerColor;
    private const int BoardSize = 8;
    private Dictionary<string, Coords[]> _safeSquares;
    private CheckState _checkState = new(false);

    public char[][] ChessBoardView =>
        _chessBoard
            .AsParallel()
            .WithDegreeOfParallelism(3)
            .Select(row => row.Select(piece => piece?.FenChar ?? default).ToArray())
            .ToArray();

    public ChessBoard(char[][]? state = null, Color? color = null)
    {
        _chessBoard = state is null ? NewBoard() : LoadFromChar(state);
        _playerColor = color.GetValueOrDefault();
        _safeSquares = FindSafeSquares();
    }

    public void Move(int prevX, int prevY, int newX, int newY)
    {
        Piece? piece = _chessBoard[prevX][prevY];
        if (piece is null || piece.Color != _playerColor) return;

        var pieceSafeSquares = _safeSquares[$"{prevX},{prevY}"];
        if (!pieceSafeSquares.Any(coords => coords.X == newX && coords.Y == newY))
        {
            Console.WriteLine("Square is not safe");
            return;
        }

        if (piece is Pawn or King or Rook && piece.HasMoved)
        {
            piece.HasMoved = true;
        }

        _chessBoard[prevX][prevY] = null;
        _chessBoard[newX][newY] = piece;
        _playerColor = _playerColor == Color.White ? Color.Black : Color.White;

        IsInCheck(_playerColor, true);
        _safeSquares = FindSafeSquares();
    }

    private Dictionary<string, Coords[]> FindSafeSquares()
        {
            var safeSquares = new Dictionary<string, Coords[]>();

            for (int x = 0; x < BoardSize; x++)
            {
                for (int y = 0; y < BoardSize; y++)
                {
                    var piece = _chessBoard[x][y];

                    if (piece is null || piece.Color != _playerColor) continue;

                    List<Coords> pieceSafeSquares = new();

                    foreach (var (dx, dy) in piece.Directions)
                    {
                        var newX = x + (piece.Color == Color.Black ? -1 : 1) * dx;
                        var newY = y + (piece.Color == Color.Black ? -1 : 1) * dy;

                        if (!AreCoordsValid(newX, newY)) continue;
                        var newPiece = _chessBoard[newX][newY];
                        if (newPiece is not null && newPiece.Color == piece.Color) continue;

                        if (piece is Pawn)
                        {
                            switch (dx)
                            {
                                case 2 or -2 when newPiece is not null:
                                case 2 or -2 when _chessBoard[newX + (dx == 2 ? -1 : 1)][newY] is not null:
                                case 1 or -1 when dy == 0 && newPiece is not null:
                                    continue;
                            }

                            if (dy is 1 or -1 && (newPiece is null || newPiece.Color == piece.Color)) continue;
                        }

                        if (piece is Pawn or Knight or King)
                        {
                            if (IsPositionSafeAfterMove(piece, x, y, newX, newY))
                            {
                                pieceSafeSquares.Add(new Coords(newX, newY));
                            }
                        }
                        else
                        {
                            while (AreCoordsValid(newX, newY))
                            {
                                newPiece = _chessBoard[newX][newY];
                                if (newPiece is not null && newPiece.Color == piece.Color) break;
                                if (IsPositionSafeAfterMove(piece, x, y, newX, newY))
                                {
                                    pieceSafeSquares.Add(new Coords(newX, newY));
                                }

                                if (newPiece != null) break;
                                newX += dx;
                                newY += dy;
                            }
                        }
                    }

                    if (pieceSafeSquares.Any()) safeSquares.Add($"{x},{y}", pieceSafeSquares.ToArray());
                }
            }

            return safeSquares;
        }

    private bool IsInCheck(Color playerColor, bool checkingCurrentPosition)
    {
        for (int x = 0; x < BoardSize; x++)
        {
            for (int y = 0; y < BoardSize; y++)
            {
                var piece = _chessBoard[x][y];

                if (piece is null || piece.Color == playerColor) continue;

                foreach (var (dx, dy) in piece.Directions)
                {
                    var newX = x + dx;
                    var newY = y + dy;

                    if (!AreCoordsValid(newX, newY)) continue;

                    if (piece is Pawn or Knight or King)
                    {
                        if (piece is Pawn && dy == 0) continue;

                        var attackedPiece = _chessBoard[newX][newY];

                        if (attackedPiece is not King || attackedPiece.Color != playerColor) continue;

                        if (checkingCurrentPosition) _checkState = new CheckState(true, newX, newY);
                        return true;
                    }

                    while (AreCoordsValid(newX, newY))
                    {
                        var attackedPiece = _chessBoard[newX][newY];
                        if (attackedPiece is King && attackedPiece.Color == playerColor)
                        {
                            if (checkingCurrentPosition) _checkState = new CheckState(true, newX, newY);
                            return true;
                        }

                        if (attackedPiece != null) break;

                        newX += dx;
                        newY += dy;
                    }
                }
            }
        }

        if (checkingCurrentPosition) _checkState = new CheckState(false);
        return false;
    }

    private bool IsPositionSafeAfterMove(Piece? piece, int prevX, int prevY, int newX, int newY)
    {
        var newPiece = _chessBoard[newX][newY];
        if (piece is null) return false;

        if (newPiece is not null && newPiece.Color == piece.Color) return false;

        // Simulate position;
        _chessBoard[prevX][prevY] = null;
        _chessBoard[newX][newY] = piece;
        bool isPositionSafe = !IsInCheck(piece.Color, false);

        // Restore position back;
        _chessBoard[prevX][prevY] = piece;
        _chessBoard[newX][newY] = newPiece;
        return isPositionSafe;
    }

    private static bool AreCoordsValid(int x, int y) => x >= 0 && y >= 0 && x < BoardSize && y < BoardSize;

    private static Piece?[][] NewBoard()
    {
        var board = new Piece?[8][];
        for (int i = 0; i < 8; i++)
            board[i] = new Piece?[8];

        InitializePieces(board, 0, Color.White);
        InitializePawns(board, 1, Color.White);

        InitializePieces(board, 7, Color.Black);
        InitializePawns(board, 6, Color.Black);

        return board;
    }

    private static void InitializePieces(Piece?[][] board, int row, Color color)
    {
        board[row] = new Piece?[]
        {
            new Rook(color), new Knight(color), new Bishop(color), new Queen(color),
            new King(color), new Bishop(color), new Knight(color), new Rook(color)
        };
    }

    private static void InitializePawns(IReadOnlyList<Piece?[]> board, int row, Color color)
    {
        for (int col = 0; col < 8; col++)
            board[row][col] = new Pawn(color);
    }

    private static Piece?[][] LoadFromChar(char[][] view)
    {
        var result = new Piece?[view.Length][];

        for (int i = 0; i < view.Length; i++)
        {
            result[i] = new Piece?[view[i].Length];
            for (int j = 0; j < view[i].Length; j++)
            {
                var fenChar = view[i][j];
                var piece = CharToPiece(fenChar);
                result[i][j] = piece;
            }
        }

        return result;
    }

    private static Piece? CharToPiece(char fenChar)
        => fenChar switch
        {
            FenChar.WhiteRook => new Rook(Color.White),
            FenChar.WhiteKnight => new Knight(Color.White),
            FenChar.WhiteBishop => new Bishop(Color.White),
            FenChar.WhiteQueen => new Queen(Color.White),
            FenChar.WhiteKing => new King(Color.White),
            FenChar.WhitePawn => new Pawn(Color.White),
            FenChar.NiggaRook => new Rook(Color.Black),
            FenChar.NiggaKnight => new Knight(Color.Black),
            FenChar.NiggaBishop => new Bishop(Color.Black),
            FenChar.NiggaQueen => new Queen(Color.Black),
            FenChar.NiggaKing => new King(Color.Black),
            FenChar.NiggaPawn => new Pawn(Color.Black),
            _ => null,
        };
}