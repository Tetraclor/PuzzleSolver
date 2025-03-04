using PuzzleSolver.Core.Primitives;

namespace PuzzleSolver.Core.Solvers;

public class TetrisPuzzleSolver : ITetrisPuzzleSolver
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

        ulong iterations = 0;
        ulong steps = 0;

        Req(board, 0);

        Console.WriteLine($"Все заполенные варианты: {++iterations}");
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
                var copyBrick = TetrisPuzzle.Shift(permut.Copy(), currentPoint);

                if (board.IsPossiblePlace(copyBrick) is false)
                {
                    continue;
                }

                var copyBoard = board.Copy();

                copyBoard.TryPlace(copyBrick);

                Req(copyBoard, pointIndex + 1);
            }

            // Вариант, ничего не вставлять. 
            Req(board.Copy(), pointIndex + 1);
        }
    }
}
