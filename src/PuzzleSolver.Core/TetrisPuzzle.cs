using Microsoft.VisualBasic;
using PuzzleSolver.Core.Abstract;
using PuzzleSolver.Core.Primitives;

namespace PuzzleSolver.Core;

public class TetrisPuzzle
{
    /// <summary>
    /// **
    ///  **
    /// </summary>
    public readonly static Brick BrickLadder = CreateBrickFromString("Ladder", @"
**
 **
");

    /// <summary>
    /// ****
    /// </summary>
    public readonly static Brick BrickLine = CreateBrickFromString("Line", @"
****
");

    /// <summary>
    ///  *
    /// ***
    /// </summary>
    public readonly static Brick BrickRoof = CreateBrickFromString("Roof", @"
***
 *
");

    /// <summary>
    /// *
    /// ***
    /// </summary>
    public readonly static Brick BrickL = CreateBrickFromString("L", @"
*
***
");

    // TODO   .

    static TetrisPuzzle()
    {
        AllBrickTypes.Add(BrickLadder);
        AllBrickTypes.Add(BrickLine);
        AllBrickTypes.Add(BrickRoof);

        foreach (var brick in AllBrickTypes)
        {
            brick.MinBorder = GetMinPoint(brick);
            brick.MaxBorder = GetMaxPoint(brick);
        }
    }

    public static List<Brick> AllBrickTypes = [];

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

        brick.MinBorder += shift;
        brick.MaxBorder += shift;

        return brick;
    }

    public static Brick Rotate(Brick brick, Point centr, int angleDegree)
    {
        if (angleDegree % 90 != 0)
        {
            throw new ArgumentException("������ ���� ������ 90", nameof(angleDegree));
        }

        for(var i = 0; i < brick.Points.Length; i++)
        {
            var point = brick.Points[i];

            var radius = Length(centr, point);

            if (radius == 0)
            {
                continue; // ���������� �����, ����������� � �������
            }

            var angle = angleDegree * Math.PI / 180;
            var currentAngle = Math.Atan2(point.Y - centr.Y, point.X - centr.X);
            var resultAngle = angle + currentAngle;

            var x = centr.X + radius * Math.Cos(resultAngle);
            var y = centr.Y + radius * Math.Sin(resultAngle);

            x = Math.Round(x);
            y = Math.Round(y);

            brick.Points[i] = new Point((int)x, (int)y);
        }

        brick.MinBorder = GetMinPoint(brick);
        brick.MaxBorder = GetMaxPoint(brick);

        return brick;
    }

    public static Brick CreateBrickFromString(string type, string brickString, char brickChar = '*')
    {
        var lines = brickString
            .Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
            .Where(line => !string.IsNullOrEmpty(line))
            .ToArray();

        var points = new List<Point>();
        var minX = int.MaxValue;

        // Сначала собираем все точки в порядке слева направо, сверху вниз
        for (var y = 0; y < lines.Length; y++)
        {
            var line = lines[y];
            for (var x = 0; x < line.Length; x++)
            {
                if (line[x] == brickChar)
                {
                    points.Add(new Point(x, y));
                    minX = Math.Min(minX, x);
                }
            }
        }

        var brick = new Brick(points.ToArray());
        brick.MinBorder = GetMinPoint(brick);
        brick.MaxBorder = GetMaxPoint(brick);
        brick.Type = type;
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

            yield return Shift(rotated.Copy(), delta);

            rotated = Rotate(rotated.Copy(), point, 90);

            yield return Shift(rotated.Copy(), delta);

            rotated = Rotate(rotated.Copy(), point, 90);

            yield return Shift(rotated.Copy(), delta);
        }
    }

    public static Point GetMinPoint(Brick brick)
    {
        var based = new Point(int.MaxValue, int.MaxValue);

        foreach (var point in brick.Points)
        {
            based.X = Math.Min(point.X, based.X);
            based.Y = Math.Min(point.Y, based.Y);
        }

        return based;
    }
    public static Point GetMaxPoint(Brick brick)
    {
        var based = new Point(int.MinValue, int.MinValue);

        foreach (var point in brick.Points)
        {
            based.X = Math.Max(point.X, based.X);
            based.Y = Math.Max(point.Y, based.Y);
        }

        return based;
    }

    public static bool CheckSolver(ITetrisPuzzleSolver tetrisPuzzleSolver)
    {
        var board = new Board(new Point(4, 4));
        var pool = new List<Brick>() { BrickRoof, BrickRoof, BrickRoof, BrickRoof };
        var result = tetrisPuzzleSolver.Solve(new SolveArguments(board, pool));

        if (result.Boards.Count() != 2)
        {
            return false;
        }

        return true;
    }
}
