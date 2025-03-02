using PuzzleSolver.Core.Primitives;

namespace PuzzleSolver.Core;

public class TetrisPuzzleSolver3 : ITetrisPuzzleSolver
{
    public IEnumerable<Board> Solve(Board board, List<Brick> pool)
    {
        var permutations = pool
            .SelectMany(brick => TetrisPuzzle.Permutations(brick).ToArray())
            .Distinct()
            .ToArray();

        var allPoints = board.GetAllPoints().ToArray();
        var solved = new List<Board>(); // Используется для уникальных решений

        ulong iterations = 0;
        ulong steps = 0;
        var parallelOptions = new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount };

        Req(board, 0);

        Console.WriteLine($"Все заполненные варианты: {iterations}");
        Console.WriteLine($"Шагов сделано: {steps}");

        return solved;

        void Req(Board currentBoard, int pointIndex)
        {
            Interlocked.Increment(ref steps);

            if (pointIndex == allPoints.Length)
            {
                Interlocked.Increment(ref iterations);
                if (iterations % 100_000 == 0)
                {
                    Console.WriteLine(Interlocked.Read(ref steps));
                }
                if (currentBoard.IsFilled2())
                {
                    if (!solved.Contains(currentBoard))
                    {
                        solved.Add(currentBoard);
                    }
                }
                return;
            }

            var currentPoint = allPoints[pointIndex];

            if (currentBoard[currentPoint] is not null)
            {
                return;
            }

            var actions = new List<Action>();

            foreach (var permut in permutations)
            {
                actions.Add(() =>
                {
                    var shiftedBrick = TetrisPuzzle.Shift(permut.Copy(), currentPoint);
                    if (!currentBoard.IsPossiblePlace(shiftedBrick)) return;

                    var boardCopy = currentBoard.Copy();
                    boardCopy.UnsafePlace(shiftedBrick);
                    Req(boardCopy, pointIndex + 1);
                });
            }

            // Вариант без размещения кирпича
            actions.Add(() =>
            {
                var boardCopy = currentBoard.Copy();
                Req(boardCopy, pointIndex + 1);
            });

            Parallel.Invoke(parallelOptions, [..actions]);
        }
    }
}
