using Chess.Core.Logic.Constants;

namespace Chess.Core.Logic.Pieces;

public   class Piece
{
    protected virtual char FenChar;
    protected virtual Coords[] Directions();

    public Piece(Color pieceColor)
    {
        PieceColor = pieceColor;
    }
    
    public Color PieceColor { get; }
}