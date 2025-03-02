using PuzzleSolver.Core.Primitives;

namespace PuzzleSolver.Core;

public class TetrisPuzzleSolver4 : ITetrisPuzzleSolver
{
    public IEnumerable<Board> Solve(Board board, List<Brick> pool)
    {
        var permutations = pool
            .SelectMany(brick =>
            {
                return TetrisPuzzle
                    .Permutations(brick)
                    .ToArray();
            })
            .Distinct()
            .ToArray();

        var allPoints = board.GetAllPoints().ToArray();
        var solved = new List<Board>();
        var hashed = new HashSet<Board>();

        ulong iterations = 0;
        ulong steps = 0;

        Parallel.Invoke([
            () => Req(board, 0),
            () => Req(board, 1)
        ]);

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
                if (board.IsFilled2())
                {
                    hashed.Add(board);
                    if (!solved.Contains(board))
                    {
                        Console.WriteLine("Новое решение найдено.");
                        solved.Add(board);
                    }
                }
                return;
            }

            var currentPoint = allPoints[pointIndex];

            if (board[currentPoint] is not null)
            {
                return;
            }

            foreach (var permut in permutations)
            {
                var copyPermut = permut.Copy();
                var shiftBrick = TetrisPuzzle.Shift(copyPermut, currentPoint);

                if (board.IsPossiblePlace(shiftBrick) is false)
                {
                    continue;
                }

                var copyBoard = board.Copy();

                copyBoard.UnsafePlace(shiftBrick);

                Req(copyBoard, pointIndex + 1);
            }

            if (pointIndex != 0)
            {
                // Вариант, ничего не вставлять. 
                Req(board.Copy(), pointIndex + 1);
            }

        }
    }
}

