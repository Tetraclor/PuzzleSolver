using System.Drawing;
using System.Text;

namespace PuzzleSolver.Core.Primitives;

public class Board
{
    public Point Size;
    public Brick[,] Field;
    public int FilledCount = 0;
    public List<Brick> Bricks = [];

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
        var newBoard = new Board()
        {
            Size = Size,
            Field = (Brick[,]) Field.Clone(),
            Bricks = [..Bricks],
            FilledCount = FilledCount,
        };

        return newBoard;
    }

    public bool IsFilled()
    {
        return !GetAllPoints().Any(v => this[v] is null);
    }

    public bool IsFilled2()
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

        Bricks.Add(brick);

        foreach (var point in brick.Points)
        {
            this[point] = brick;
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

        if (Bricks.Count != other.Bricks.Count)
            return false;

        //foreach (var brick in Bricks)
        //{
        //    if (other.Bricks.Contains(brick) is false)
        //        return false;
        //}

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

    int? cachedHashCode = null;

    public override int GetHashCode()
    {
        return PrintBoardForHashCode(this).GetHashCode();
    }

    static string PrintBoardForHashCode(Board board)
    {
        var stringBuilder = new StringBuilder();

        var exists = new Dictionary<Brick, char>();
        var currentChar = 'a';

        for (var y = 0; y < board.Size.Y; y++)
        {
            for (var x = 0; x < board.Size.X; x++)
            {
                if (board.Field[y, x] is not null)
                {
                    var brick = board.Field[y, x];
                    if (!exists.TryGetValue(brick, out char value))
                    {
                        exists[brick] = ++currentChar;
                    }
                    stringBuilder.Append(value);

                }
                else
                {
                    stringBuilder.Append(' ');
                }
            }

            stringBuilder.AppendLine();
        }

        return stringBuilder.ToString();
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