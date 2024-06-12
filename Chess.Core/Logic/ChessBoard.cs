using Chess.Core.Logic.Pieces;

namespace Chess.Core.Logic;

public class ChessBoard
{
    private readonly Piece?[][] _chessBoard;
    private const Color PlayerColor = Color.White;
    private const int BoardSize = 8;
    private Dictionary<string, Coords[]> _safeSquares;
    private CheckState _checkState = new(false);

    public string[][] ChessBoardView
    {
        get
        {
            return _chessBoard
                .Select(row => row.Select(piece => piece is Piece ? piece.FenChar.ToString() : null).ToArray())
                .ToArray();
        }
    }

    public ChessBoard()
    {
        _chessBoard =
            new[]
            {
                new Piece[]
                {
                    new Rook(Color.White),

                    new Knight(Color.White), new Bishop(Color.White), new Queen(Color.White),
                    new King(Color.White), new Bishop(Color.White), new Knight(Color.White), new Rook(Color.White)
                },

                new Piece[]
                {
                    new Pawn(Color.White),
                    new Pawn(Color.White), new Pawn(Color.White), new Pawn(Color.White),
                    new Pawn(Color.White), new Pawn(Color.White), new Pawn(Color.White), new Pawn(Color.White)
                },
                new Piece[]
                {
                    null,
                    null, null, null, null, null, null, null
                },
                new Piece[]
                {
                    null,
                    null, null, null, null, null, null, null
                },
                new Piece[]
                {
                    null,
                    null, null, null, null, null, null, null
                },
                new Piece[]
                {
                    null,
                    null, null, null, null, null, null, null
                },
                new Piece[]
                {
                    new Pawn(Color.Nigga),

                    new Pawn(Color.Nigga), new Pawn(Color.Nigga), new Pawn(Color.Nigga),
                    new Pawn(Color.Nigga), new Pawn(Color.Nigga), new Pawn(Color.Nigga), new Pawn(Color.Nigga)
                },
                new Piece[]
                {
                    new Rook(Color.Nigga),

                    new Knight(Color.Nigga), new Bishop(Color.Nigga), new Queen(Color.Nigga),
                    new King(Color.Nigga), new Bishop(Color.Nigga), new Knight(Color.Nigga), new Rook(Color.Nigga)
                }
            }
            ;
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

                if (piece is null || piece.Color != PlayerColor) continue;

                List<Coords> pieceSafeSquares = new();
                ;

                foreach (var (dx, dy) in piece.Directions)
                {
                    var newX = x + dx;
                    var newY = y + dy;

                    if (!AreCoordsValid(newX, newY)) continue;
                    var newPiece = _chessBoard[newX][newY];
                    if (newPiece is not null && newPiece.Color == piece.Color) continue;

                    if (piece is Pawn)
                    {
                        if (dx == 2 || dx == -2)
                        {
                            if (newPiece is not null) continue;
                            if (_chessBoard[newX + (dx == 2 ? -1 : 1)][newY] is not null) continue;
                        }

                        if ((dx == 1 || dx == -1) && dy == 0 && newPiece is not null) continue;

                        if ((dy == 1 || dy == -1) && (newPiece is not null || piece.Color == newPiece?.Color)) continue;
                    }

                    if (piece is Pawn || piece is Knight || piece is King)
                    {
                        if (IsPositionSafeAfterMove(piece, x, y, newX, newY))
                        {
                            pieceSafeSquares.Add(new Coords(newX, newY));
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
        }

        return safeSquares;
    }

    public bool IsInCheck(Color playerColor, bool checkingCurrentPosition)
    {
        for (int x = 0; x < BoardSize; x++)
        {
            for (int y = 0; y < BoardSize; y++)
            {
                var piece = _chessBoard[x][y];

                if (piece is null || piece.Color != PlayerColor) continue;

                foreach (var (dx, dy) in piece.Directions)
                {
                    var newX = x + dx;
                    var newY = y + dy;

                    if (!AreCoordsValid(newX, newY)) continue;
                    if (piece is Pawn || piece is Knight || piece is King)
                    {
                        if (piece is Pawn && dy == 0) continue;

                        var attackedPiece = _chessBoard[newX][newY];

                        if (attackedPiece is King && attackedPiece.Color == playerColor)
                        {
                            if (checkingCurrentPosition) this._checkState = new CheckState(true, newX, newY);
                            return true;
                        }
                    }
                    else
                    {
                        while (AreCoordsValid(newX, newY))
                        {
                            var attackedPiece = _chessBoard[newX][newY];
                            if (attackedPiece is King && attackedPiece.Color == playerColor)
                            {
                                if (checkingCurrentPosition) this._checkState = new CheckState(true, newX, newY);
                                return true;
                            }

                            if (attackedPiece is null) break;

                            newX += dx;
                            newY += dy;
                        }
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

        //Simulate position;
        _chessBoard[prevX][prevY] = null;
        _chessBoard[newX][newY] = piece;
        bool isPositionSafe = !IsInCheck(piece.Color, false);

        //restore position back;
        _chessBoard[prevX][prevY] = piece;
        _chessBoard[newX][newY] = piece;
        return isPositionSafe;
    }

    private static bool AreCoordsValid(int x, int y) => x >= 0 && y >= 0 && x < BoardSize && y > BoardSize;
}