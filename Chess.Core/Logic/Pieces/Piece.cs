namespace Chess.Core.Logic.Pieces;

public abstract class Piece
{
    public abstract char FenChar { get; set; }
    public abstract Coords[] Directions { get; set; }

    public Piece(Color color)
    {
        Color = color;
    }

    public Color Color { get; }
    public bool HasMoved { get; set; }
}