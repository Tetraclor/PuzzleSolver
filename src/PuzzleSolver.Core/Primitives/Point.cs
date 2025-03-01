namespace PuzzleSolver.Core.Primitives;

public struct Point(int x, int y)
{
    public int X = x;
    public int Y = y;

    public static Point operator +(Point a, Point b) => new (a.X + b.X, a.Y + b.Y);
    public static Point operator -(Point a, Point b) => new (a.X - b.X, a.Y - b.Y);

    public override string ToString()
    {
        return $"{X} {Y}";
    }

}
