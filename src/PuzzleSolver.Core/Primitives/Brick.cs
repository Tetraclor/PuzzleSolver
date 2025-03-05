using System.Collections.Immutable;
using System.Linq;

namespace PuzzleSolver.Core.Primitives;

public class Brick : IEquatable<Brick>
{
    public string Type;
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
            Type = Type,
            MinBorder = MinBorder,
            MaxBorder = MaxBorder,
        };
    }

    public bool IsSameShape(Brick other)
    {
        if (Type is not null && other.Type is not null)
            return Type == other.Type;

        if (other == null || Points == null || other.Points == null || Points.Length != other.Points.Length)
            return false;

        // Нормализуем обе фигуры к началу координат (0,0)
        var normalizedThis = NormalizeBrick(this);
        var normalizedOther = NormalizeBrick(other);

        // Проверяем все возможные повороты
        for (int angle = 0; angle < 360; angle += 90)
        {
            var rotated = TetrisPuzzle.Rotate(normalizedThis.Copy(), new Point(0, 0), angle);
            var normalizedRotated = NormalizeBrick(rotated);

            if (ArePointsEqual(normalizedRotated.Points, normalizedOther.Points))
                return true;
        }

        return false;
    }

    private static Brick NormalizeBrick(Brick brick)
    {
        var minX = brick.Points.Min(p => p.X);
        var minY = brick.Points.Min(p => p.Y);
        var shift = new Point(-minX, -minY);
        
        return TetrisPuzzle.Shift(brick.Copy(), shift);
    }

    private static bool ArePointsEqual(Point[] points1, Point[] points2)
    {
        if (points1.Length != points2.Length)
            return false;

        var ordered1 = points1.OrderBy(p => p.X).ThenBy(p => p.Y).ToArray();
        var ordered2 = points2.OrderBy(p => p.X).ThenBy(p => p.Y).ToArray();

        for (int i = 0; i < ordered1.Length; i++)
        {
            if (ordered1[i].X != ordered2[i].X || ordered1[i].Y != ordered2[i].Y)
                return false;
        }

        return true;
    }

    public override bool Equals(object obj)
    {
        return Equals(obj as Brick); // Приводим к Brick и используем метод IEquatable
    }

    public bool Equals(Brick other)
    {
        if (other == null) return false;

        // Сравниваем ссылки
        if (ReferenceEquals(this, other)) return true;

        // Проверяем длину массивов
        if (Points == null || other.Points == null)
            return Points == other.Points;

        if (Points.Length != other.Points.Length)
            return false;

        // Создаем множество для быстрого сравнения
        // Поэлементное сравнение массивов
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
