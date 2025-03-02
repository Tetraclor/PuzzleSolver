using System.Diagnostics.CodeAnalysis;

namespace PuzzleSolver.Core.Primitives;

public class Brick : IEquatable<Brick>
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
