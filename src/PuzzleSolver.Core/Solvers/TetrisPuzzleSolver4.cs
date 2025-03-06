using PuzzleSolver.Core.Abstract;
using PuzzleSolver.Core.Primitives;
using PuzzleSolver.Core.Permutations;
using System.Diagnostics;

namespace PuzzleSolver.Core.Solvers;

public class TetrisPuzzleSolver4 : ITetrisPuzzleSolver
{
    public SolveResult Solve(SolveArguments solveArguments)
    {
        var board = solveArguments.Board;
        var pool = solveArguments.Pool;
        
        var solved = new List<Board>();
        var steps = 0;

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
        var hashed = new HashSet<Board>();

        ulong iterations = 0;

        Parallel.Invoke([
            () => Req(board, 0),
            () => Req(board, 1)
        ]);

        Console.WriteLine($"Все конечные варианты: {++iterations}");
        Console.WriteLine($"Шагов сделано: {steps}");

        return new SolveResult(solved, steps);

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

