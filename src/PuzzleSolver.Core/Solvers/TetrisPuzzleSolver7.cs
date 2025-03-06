using PuzzleSolver.Core.Primitives;
using PuzzleSolver.Core.Permutations;
using System.Diagnostics;
using PuzzleSolver.Core.Abstract;

namespace PuzzleSolver.Core.Solvers;

public class TetrisPuzzleSolver7 : ITetrisPuzzleSolver
{
    HashSet<Board> Solved = [];

    public SolveResult Solve(SolveArguments solveArguments)
    {
        Solved = [];

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
        if (state.PointIndex == state.AllPoints.Length)
        {
            if (state.Board.IsFilled())
            {
                if (Solved.Contains(state.Board))
                {
                    yield break;
                }
                var copyBoard = state.Board.Copy();
                Solved.Add(copyBoard);
                yield return copyBoard;
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

            state.Board.UnsafePlace(permut);

            var placed = state.RemainingBricks.First(v => v.IsSameShape(permut));
            state.RemainingBricks.Remove(placed);

            var nextState = new SolverState(
                state.Board,
                state.PointIndex + 1,
                state.RemainingBricks,
                state.Steps,
                state.Stopwatch,
                state.BoardPermutations,
                state.AllPoints
            );

            foreach (var solution in FindSolutions(nextState))
            {
                yield return solution;
            }

            state.Board.UnsafeRemove(permut);
            state.RemainingBricks.Add(placed);
        }
    }
} 
