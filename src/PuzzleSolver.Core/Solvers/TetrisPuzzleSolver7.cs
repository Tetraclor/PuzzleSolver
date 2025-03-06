using PuzzleSolver.Core.Primitives;
using PuzzleSolver.Core.Permutations;
using System.Diagnostics;
using PuzzleSolver.Core.Abstract;

namespace PuzzleSolver.Core.Solvers;

public class TetrisPuzzleSolver7 : ITetrisPuzzleSolver
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
        var stopwatch = Stopwatch.StartNew();

        var initialState = new SolverState(board, 0, [.. pool], 0, stopwatch, boardPermutations, allPoints);
        var solutions = FindSolutions(initialState);

        stopwatch.Stop();

        return new SolveResult(solutions, 0);
    }

    private IEnumerable<Board> FindSolutions(SolverState state)
    {
        if (++state.Steps % 5_000_000 == 0)
        {
            Console.WriteLine($"{state.Stopwatch.Elapsed}. Итераций: {state.Steps}. {state.Steps / state.Stopwatch.Elapsed.TotalSeconds} в сек.");
            Console.WriteLine(state.Board);
        }

        if (state.PointIndex == state.AllPoints.Length)
        {
            if (state.Board.IsFilled())
            {
                yield return state.Board;
            }
            yield break;
        }

        var currentPoint = state.AllPoints[state.PointIndex];

        if (state.Board[currentPoint] is not null)
        {
            state.PointIndex++;
            foreach (var solution in FindSolutions(state))
            {
                yield return solution;
            }
            yield break;
        }

        foreach (var permut in state.BoardPermutations[currentPoint.Y, currentPoint.X]
            .BrickPermutations
            .Where(v => state.RemainingBricks.Contains(v.OriginalBrick))
            .SelectMany(v => v.Variants))
        {
            if (state.Board.IsPossiblePlace(permut) is false)
            {
                continue;
            }

            var copyBoard = state.Board.Copy();
            copyBoard.UnsafePlace(permut);

            var placed = state.RemainingBricks.First(v => v.IsSameShape(permut));
            var copyRemainingBricks = state.RemainingBricks.ToList();
            copyRemainingBricks.Remove(placed);

            // Можно избавиться от копирования доски, если перед итерацией вставлять.
            // state.PointIndex++;
            var nextState = new SolverState(
                copyBoard,
                state.PointIndex + 1,
                copyRemainingBricks,
                state.Steps,
                state.Stopwatch,
                state.BoardPermutations,
                state.AllPoints
            );

            foreach (var solution in FindSolutions(nextState))
            {
                yield return solution;
            }

            // А после итерации удалять фигуру из доски.
            // state.PointIndex--;
        }
    }
} 
