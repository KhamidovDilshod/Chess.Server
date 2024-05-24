using Chess.Core.Logic.Constants;

namespace Chess.Core.Logic.Pieces;

public class King : Piece
{
    private bool _hasMoved = false;

    public King(Color pieceColor) : base(pieceColor)
    {
    }

    protected override char FenChar() =>
        PieceColor == Color.White ? Constants.FenChar.WhiteKing : Constants.FenChar.NiggaKing;

    protected override Coords[] Directions()
        => new[]
        {
            new Coords(0, 1),
            new Coords(0, -1),
            new Coords(1, 0),
            new Coords(1, -1),
            new Coords(1, 1),
            new Coords(-1, 0),
            new Coords(-1, 1),
            new Coords(-1, -1)
        };
}