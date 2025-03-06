using System.Drawing;
using System.Text;

namespace PuzzleSolver.Core.Primitives;

public class Board
{
    public Point Size;
    public Brick[,] Field;
    public int FilledCount = 0;

    public Brick this[Point point]
    {
        get { return Field[point.Y,point.X]; }
        set { Field[point.Y, point.X] = value; }
    }

    public Board(Point size)
    {
        Size = size;
        Field = new Brick[size.Y,size.X];
    }

    private Board()
    {
    }

    public Board Copy()
    {
        var filed = (Brick[,])Field.Clone();

        var newBoard = new Board()
        {
            Size = Size,
            Field = filed,
            FilledCount = FilledCount,
        };

        return newBoard;
    }

    public bool IsFilled()
    {
        return FilledCount == Size.X * Size.Y;
    }

    public IEnumerable<Point> GetAllPoints()
    {
        for (var y = 0; y < Size.Y; y++)
        {
            for (var x = 0; x < Size.X; x++)
            {
                yield return new Point(x, y);
            }
        }
    }

    public bool IsPossiblePlace(Brick brick)
    {
        if (brick.MinBorder.X < 0 || 
            brick.MinBorder.Y < 0 ||
            brick.MaxBorder.X >= Size.X ||
            brick.MaxBorder.Y >= Size.Y)
        {
            return false;
        }

        foreach (var p in brick.Points)
        {
            if (this[p] is not null)
            {
                return false;
            }
        }

        return true;

    }

    public bool TryPlace(Brick brick)
    {
        if (IsPossiblePlace(brick) is false)
        {
            return false;
        }

        UnsafePlace(brick);

        return true;
    }

    public void UnsafePlace(Brick brick)
    {
        FilledCount += brick.Points.Length;

        foreach (var point in brick.Points)
        {
            this[point] = brick;
        }
    }

    public void UnsafeRemove(Brick brick)
    {
        FilledCount -= brick.Points.Length;
        foreach (var point in brick.Points)
        {
            this[point] = null;
        }
    }

    public override bool Equals(object obj)
    {
        return Equals(obj as Board);
    }

    public bool Equals(Board other)
    {
        if (other is null)
            return false;

        for (var y = 0; y < Size.Y; y++)
        {
            for (var x = 0; x < Size.X; x++)
            {
                if (this[new Point(x, y)]?.Equals(other[new Point(x, y)]) is false)
                    return false;
            }
        }


        return true;
    }

    public override int GetHashCode()
    {
        var hash = new HashCode();
        // Incorporate board size and filled count.
        hash.Add(Size);
        hash.Add(FilledCount);

        // Iterate in row-major order.
        for (int y = 0; y < Size.Y; y++)
        {
            for (int x = 0; x < Size.X; x++)
            {
                // Include Brick hash, using 0 for null.
                hash.Add(this[new Point(x, y)]?.GetHashCode() ?? 0);
            }
        }

        return hash.ToHashCode();
    }

    static string PrintBoard(Board board)
    {
        var stringBuilder = new StringBuilder();
        var emptyChar = ' ';
        var horizontalBorderChar = '-';
        var verticalBorderChar = '|';
        var cornerChar = '+';

        var exists = new Dictionary<Brick, char>();
        var currentChar = 'a';

        for (var y = -1; y <= board.Size.Y; y++)
        {
            for (var x = -1; x <= board.Size.X; x++)
            {
                if ((y == -1 || y == board.Size.Y) && (x == -1 || x == board.Size.X))
                {
                    // Углы
                    stringBuilder.Append(cornerChar);
                }
                else if (y == -1 || y == board.Size.Y)
                {
                    // Горизонтальные границы
                    stringBuilder.Append(horizontalBorderChar);
                }
                else if (x == -1 || x == board.Size.X)
                {
                    // Вертикальные границы
                    stringBuilder.Append(verticalBorderChar);
                }
                else if (board.Field[y, x] is not null)
                {
                    var brick = board.Field[y, x];
                    if (!exists.ContainsKey(brick))
                    {
                        exists[brick] = ++currentChar;
                    }
                    stringBuilder.Append(exists[brick]);

                }
                else
                {
                    stringBuilder.Append(emptyChar);
                }
            }

            stringBuilder.AppendLine();
        }

        return stringBuilder.ToString();
    }

    public override string ToString()
    {
        return PrintBoard(this);
    }

}