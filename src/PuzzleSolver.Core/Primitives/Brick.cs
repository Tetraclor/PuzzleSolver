using System.Diagnostics.CodeAnalysis;

namespace PuzzleSolver.Core.Primitives;

public class Brick
{
    public Point[] Points;

    public Brick Copy()
    {
        return new Brick
        {
            Points = [.. Points]
        };
    }
    public override bool Equals(object obj)
    {
        if (obj == null || GetType() != obj.GetType())
            return false;

        Brick other = (Brick)obj;

        if (Points == null && other.Points == null)
            return true;

        if (Points == null || other.Points == null)
            return false;

        if (Points.Length != other.Points.Length)
            return false;

        var pointsSet = new HashSet<Point>(Points);
        var otherPointsSet = new HashSet<Point>(other.Points);

        var result = pointsSet.SetEquals(otherPointsSet);

        return result;
    }

    public override int GetHashCode()
    {
        unchecked
        {
            int hash = 17;
            if (Points != null)
            {
                foreach (var point in Points)
                {
                    hash = hash * 23 + point.GetHashCode();
                }
            }
            return hash;
        }
    }
}
