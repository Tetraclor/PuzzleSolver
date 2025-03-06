using PuzzleSolver.Core.Primitives;
using PuzzleSolver.Core.Permutations;
using System.Diagnostics;
using PuzzleSolver.Core.Abstract;

namespace PuzzleSolver.Core.Solvers;

public class TetrisPuzzleSolver6 : ITetrisPuzzleSolver
{
    public SolveResult Solve(SolveArguments solveArguments)
    {
        var board = solveArguments.Board;
        var pool = solveArguments.Pool;
        
        var boardPermutations = new PoolPermutations[board.Size.Y, board.Size.X];

        foreach (var boardPoint in board.GetAllPoints())
        {
            boardPermutations[boardPoint.Y, boardPoint.X] = new PoolPermutations(pool, boardPoint);
        }

        var checkedPoints = new HashSet<Point>();

        foreach(var boardPoint in board.GetAllPoints())
        {
            checkedPoints.Add(boardPoint);

            var poolPermutations = new PoolPermutations(pool, boardPoint);

            foreach (var brickPermutation in poolPermutations.BrickPermutations)
            {
                foreach (var variant in brickPermutation.Variants)
                {
                    var isPossiblePlace = board.IsPossiblePlace(variant);

                    foreach (var point in variant.Points)
                    {
                        if (point.X < 0 || point.Y < 0 || point.X >= board.Size.X || point.Y >= board.Size.Y)
                        {
                            continue;
                        }

                        if (isPossiblePlace is false || checkedPoints.Contains(point) is false)
                        {
                            var permutationForPoint = boardPermutations[point.Y, point.X];

                            permutationForPoint.RemoveVariant(brickPermutation.OriginalBrick, variant);
                        }
                    }
                }
            }
        }

        var allPoints = board.GetAllPoints().ToArray();
        var solved = new HashSet<Board>();

        ulong steps = 0;

        var stopwatch = Stopwatch.StartNew();

        try
        {
            Req(board, 0, [.. pool]);
        }
        catch (ReqInterruptException)
        {
            Console.WriteLine("Поиск решений прерван.");
        }

        stopwatch.Stop();

        Console.WriteLine($"Все решения: {solved.Count}");
        Console.WriteLine($"Итераций: {steps}");

        return new SolveResult(solved, (int)steps);

        void Req(Board board, int pointIndex, List<Brick> remainingBricks)
        {
            if (++steps % 5_000_000 == 0)
            {
                Console.WriteLine($"{stopwatch.Elapsed}. Итераций: {steps}. {steps / stopwatch.Elapsed.TotalSeconds} в сек.");
                Console.WriteLine(board);
            }

            if (pointIndex == allPoints.Length)
            {
                if (board.IsFilled())
                {
                    solved.Add(board);

                    if (solved.Count >= 50)
                    {
                        throw new ReqInterruptException("Найдено более 50 решений.");
                    }
                }
                return;
            }

            var currentPoint = allPoints[pointIndex];

            if (board[currentPoint] is not null)
            {
                Req(board, pointIndex + 1, remainingBricks);
                return;
            }

            foreach (var permut in boardPermutations[currentPoint.Y, currentPoint.X]
                .BrickPermutations
                .Where(v => remainingBricks.Contains(v.OriginalBrick))
                .SelectMany(v => v.Variants))
            {
                if (board.IsPossiblePlace(permut) is false)
                {
                    continue;
                }

                var copyBoard = board.Copy();

                copyBoard.UnsafePlace(permut);

                var placed = remainingBricks.First(v => v.IsSameShape(permut));

                var copyRemainingBricks = remainingBricks.ToList();

                copyRemainingBricks.Remove(placed);

                Req(copyBoard, pointIndex + 1, copyRemainingBricks);
            }
        }
    }
} 
