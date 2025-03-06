using PuzzleSolver.Core.Abstract;
using PuzzleSolver.Core.Primitives;
using PuzzleSolver.Core.Permutations;
using System.Diagnostics;

namespace PuzzleSolver.Core.Solvers;

public class TetrisPuzzleSolver3 : ITetrisPuzzleSolver
{
    public SolveResult Solve(SolveArguments solveArguments)
    {
        var board = solveArguments.Board;
        var pool = solveArguments.Pool;
        
        var solved = new List<Board>();
        var steps = 0;

        var permutations = pool
            .SelectMany(brick => TetrisPuzzle.Permutations(brick).ToArray())
            .Distinct()
            .ToArray();

        var allPoints = board.GetAllPoints().ToArray();
        var iterations = 0;
        var parallelOptions = new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount };

        Req(board, 0);

        Console.WriteLine($"Все заполненные варианты: {iterations}");
        Console.WriteLine($"Шагов сделано: {steps}");

        return new SolveResult(solved, steps);

        void Req(Board currentBoard, int pointIndex)
        {
            steps++;

            if (pointIndex == allPoints.Length)
            {
                iterations++;
                if (iterations % 100_000 == 0)
                {
                    Console.WriteLine(steps);
                }
                if (currentBoard.IsFilled())
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
