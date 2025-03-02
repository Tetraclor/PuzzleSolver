using System.Collections.Immutable;

namespace PuzzleSolver.Core.Primitives;

public class Brick : IEquatable<Brick>
{
    public Point MinBorder;
    public Point MaxBorder;
    public Point[] Points;

    public Brick(Point[] points)
    {
        Points = [.. points];
    }

    public Brick()
    {
    }

    public Brick Copy()
    {
        return new Brick
        {
            Points = [.. Points],
            MinBorder = MinBorder,
            MaxBorder = MaxBorder,
        };
    }
    public override bool Equals(object obj)
    {
        return Equals(obj as Brick); // �������� � Brick � ���������� ����� IEquatable
    }

    public bool Equals(Brick other)
    {
        if (other == null) return false;

        // ���������� ������
        if (ReferenceEquals(this, other)) return true;

        // ��������� ����� ��������
        if (Points == null || other.Points == null)
            return Points == other.Points;

        if (Points.Length != other.Points.Length)
            return false;

        // ������� ��������� ��� �������� ���������
        // ������������ ��������� ��������
        for (int i = 0; i < Points.Length; i++)
        {
            if (other.Points.Contains(Points[i]) is false)
                return false;
        }

        return true;
    }

    //public override int GetHashCode()
    //{
    //    int pointsHashCode = 0;
    //    if (Points != null)
    //    {
    //        foreach (var point in Points
    //            .Select(v => v.GetHashCode())
    //            .OrderBy(v => v))
    //        {
    //            pointsHashCode = HashCode.Combine(pointsHashCode, point);
    //        }
    //    }

    //    return pointsHashCode;
    //}
}
