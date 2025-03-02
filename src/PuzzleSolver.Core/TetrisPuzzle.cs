using Microsoft.VisualBasic;
using PuzzleSolver.Core.Primitives;

namespace PuzzleSolver.Core;

public class TetrisPuzzle
{
    // **
    //  **
    public static Brick BrickLadder = new ()
    {
        Points =
        [
            new Point(0, 0),
            new Point(1, 0),
            new Point(1, 1),
            new Point(2, 1),
        ]
    };

    //
    // 
    public static Brick BrickLine = new()
    {
        Points =
        [
            new Point(0, 0),
            new Point(1, 0),
            new Point(2, 0),
            new Point(3, 0),
        ]
    };

    public static Brick BrickRoof = new()
    {
        Points =
        [
            new Point(0, 0),
            new Point(1, 0),
            new Point(2, 0),
            new Point(1, 1),
        ]
    };

    // TODO добавить другие фигурки.

    public readonly List<Brick> Bricks = [];

    public static double Length(Point a, Point b)
    {
        return Math.Sqrt(Math.Pow(b.X - a.X, 2) + Math.Pow(b.Y - a.Y, 2));
    }

    public static Brick Shift(Brick brick, Point shift)
    {
        for(var i = 0; i < brick.Points.Length; i++)
        {
            brick.Points[i] += shift;
        }

        return brick;
    }

    public static Brick Rotate(Brick brick, Point centr, int angleDegree)
    {
        if (angleDegree % 90 != 0)
        {
            throw new ArgumentException("ƒолжен быть кратен 90", nameof(angleDegree));
        }

        for(var i = 0; i < brick.Points.Length; i++)
        {
            var point = brick.Points[i];
            if (point.Equals(centr))
            {
                continue;
            }
            var radius = Length(centr, point);

            var angle = angleDegree * Math.PI / 180;
            var currentAgnle = Math.Acos((point.X - centr.X) / radius);
            var resultAngle = angle + currentAgnle;

            var x = centr.X + radius * Math.Cos(resultAngle);
            var y = centr.Y + radius * Math.Sin(resultAngle);

            x = Math.Round(x);
            y = Math.Round(y);

            brick.Points[i] = new Point((int)x, (int)y);
        }

        return brick;
    }


    public static bool PlacedOneFromPool(Board board, Point point, List<Brick> bricks)
    {
        foreach (var brick in bricks)
        {
            var copy = Shift(brick.Copy(), point);

            if (board.TryPlace(copy))
            {
                return true;
            }
        }
        return false;
    }

    public static IEnumerable<Brick> Permutations(Brick brick)
    {
        var firstPoint = brick.Points.First();
        foreach(var point in brick.Points)
        {
            var delta = firstPoint - point;
            yield return Shift(brick.Copy(), delta);

            var rotated = Rotate(brick.Copy(), point, 90);

            yield return Shift(rotated, delta);

            rotated = Rotate(brick.Copy(), point, 180);

            yield return Shift(rotated, delta);

            rotated = Rotate(brick.Copy(), point, 270);

            yield return Shift(rotated, delta);
        }
    }

    public static Point GetBasedPoint(Brick brick)
    {
        var based = new Point(0, 0);

        foreach (var point in brick.Points)
        {
            based.X = Math.Min(point.X, based.X);
            based.Y = Math.Min(point.Y, based.Y);
        }

        return based;
    }

    public static bool CheckSolver(ITetrisPuzzleSolver tetrisPuzzleSolver)
    {
        var board = new Board(new Point(4, 4));
        var pool = new List<Brick>() { BrickRoof };
        var result = tetrisPuzzleSolver.Solve(board, pool);

        if (result.Count() != 2)
        {
            return false;
        }

        return true;
    }
}
