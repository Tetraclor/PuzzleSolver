using PuzzleSolver.Core.Primitives;

namespace PuzzleSolver.Core.Solvers;

public class TetrisPuzzleSolver5 : ITetrisPuzzleSolver
{
    public IEnumerable<Board> Solve(Board board, List<Brick> pool)
    {
        var pool1 = pool.Distinct();

        var permutations = pool1
            .SelectMany(brick =>
            {
                return TetrisPuzzle
                    .Permutations(brick)
                    .ToArray();
            })
            .ToHashSet();

        var boardPermutations = new List<Brick>[board.Size.Y, board.Size.X];

        for (var y = 0; y < board.Size.Y; y++)
        {
            for (var x = 0; x < board.Size.X; x++)
            {
                var list = new List<Brick>();
                boardPermutations[y, x] = list;
                foreach (var permut in permutations)
                {
                    var copyPermut = permut.Copy();
                    TetrisPuzzle.Shift(copyPermut, new Point(x, y));
                    list.Add(copyPermut);
                }
            }
        }

        var checkedPoints = new HashSet<Point>();

        for (var y = 0; y < board.Size.Y; y++)
        {
            for (var x = 0; x < board.Size.X; x++)
            {
                var boardPoint = new Point(x, y);

                checkedPoints.Add(boardPoint);

                foreach (var permut in permutations)
                {
                    var copyPermut = permut.Copy();

                    TetrisPuzzle.Shift(copyPermut, boardPoint);

                    if (board.IsPossiblePlace(copyPermut) is false)
                    {
                        foreach (var point in copyPermut.Points)
                        {
                            if (point.X < 0 || point.Y < 0 || point.X >= board.Size.X || point.Y >= board.Size.Y)
                            {
                                continue;
                            }

                            var permutationForPoint = boardPermutations[point.Y, point.X];

                            permutationForPoint.Remove(copyPermut);
                        }

                        continue;
                    }

                    foreach (var point in copyPermut.Points)
                    {
                        if (point.X < 0 || point.Y < 0 || point.X >= board.Size.X || point.Y >= board.Size.Y)
                        {
                            continue;
                        }

                        if (checkedPoints.Contains(point))
                        {
                            continue;
                        }

                        var permutationForPoint = boardPermutations[point.Y, point.X];

                        permutationForPoint.Remove(copyPermut);
                    }
                }
            }
        }

        var allPoints = board.GetAllPoints().ToArray();
        var solved = new List<Board>();
        var hashed = new HashSet<Board>();

        ulong iterations = 0;
        ulong steps = 0;

        Req(board, 0);

        Console.WriteLine($"Все конечные варианты: {++iterations}");
        Console.WriteLine($"Шагов сделано: {steps}");

        return solved;

        void Req(Board board, int pointIndex)
        {
            steps++;

            if (pointIndex == allPoints.Length)
            {
                iterations++;
                if (iterations % 100_000 == 0)
                {
                    Console.WriteLine(steps);
                }
                if (board.IsFilled())
                {
                    hashed.Add(board);
                    if (!solved.Contains(board))
                    {
                        Console.WriteLine("Новое решение найдено.");
                        Console.WriteLine(board.ToString());

                        solved.Add(board);
                    }
                }
                return;
            }

            var currentPoint = allPoints[pointIndex];

            if (board[currentPoint] is not null)
            {
                Req(board, pointIndex + 1);
                return;
            }

            foreach (var permut in boardPermutations[currentPoint.Y, currentPoint.X])
            {
                if (board.IsPossiblePlace(permut) is false)
                {
                    continue;
                }

                var copyBoard = board.Copy();

                copyBoard.UnsafePlace(permut);

                Req(copyBoard, pointIndex + 1);
            }
        }
    }
}
