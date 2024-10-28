namespace Chess.Core.Logic.Pieces;

public abstract class Piece(Color color)
{
    public abstract char FenChar { get; set; }
    public abstract Coords[] Directions { get; set; }

    public Color Color { get; } = color;
    public bool HasMoved { get; set; }
}