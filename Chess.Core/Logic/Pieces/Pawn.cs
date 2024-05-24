using Chess.Core.Logic.Constants;

namespace Chess.Core.Logic.Pieces;

public class Pawn : Piece
{
    public Pawn(Color pieceColor) : base(pieceColor)
    {
    }

    protected override char FenChar() =>
        PieceColor == Color.White ? Constants.FenChar.WhitePawn : Constants.FenChar.NiggaPawn;

    protected override Coords[] Directions() =>
        new[]
        {
            new Coords(1, 0),
            new Coords(2, 0),
            new Coords(1, 1),
            new Coords(1, -1)
        };

    private void SetBlackPawnDirection()
    {
        this.Directions() = this.Directions().Select(coord => coord with { X = -1 * coord.X });
    }
}