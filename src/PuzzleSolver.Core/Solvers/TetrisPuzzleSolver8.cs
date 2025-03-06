using PuzzleSolver.Core.Primitives;
using PuzzleSolver.Core.Permutations;
using System.Diagnostics;
using PuzzleSolver.Core.Abstract;

namespace PuzzleSolver.Core.Solvers;

public class TetrisPuzzleSolver8 : ITetrisPuzzleSolver
{
    HashSet<Board> Solved = new();

    public SolveResult Solve(SolveArguments solveArguments)
    {
        Solved = new();

        var board = solveArguments.Board;
        var pool = solveArguments.Pool;

        var boardPermutations = new PoolPermutations[board.Size.Y, board.Size.X];

        foreach (var boardPoint in board.GetAllPoints())
        {
            boardPermutations[boardPoint.Y, boardPoint.X] = new PoolPermutations(pool, boardPoint);
        }

        var checkedPoints = new HashSet<Point>();

        foreach (var boardPoint in board.GetAllPoints())
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
        var stopwatch = Stopwatch.StartNew();

        var initialState = new SolverState(
            board,
            0,
            new List<Brick>(pool),
            0,
            stopwatch,
            boardPermutations,
            allPoints
        );
        // Changed: use iterative implementation to yield solutions on demand.
        var solutions = FindSolutionsIterative(initialState);

        stopwatch.Stop();

        return new SolveResult(solutions, 0);
    }

    // Remove or comment out the recursive FindSolutions method.
    // private IEnumerable<Board> FindSolutions(SolverState state)
    // {
    //    ...existing recursive implementation...
    // }

    // New iterative implementation of solution generation using yield return.
    private IEnumerable<Board> FindSolutionsIterative(SolverState initialState)
    {
        var stack = new Stack<SolverState>();
        stack.Push(initialState);

        while (stack.Count > 0)
        {
            var state = stack.Pop();
            state.Steps++;

            if (state.PointIndex >= state.AllPoints.Length)
            {
                if (state.Board.IsFilled())
                {
                    if (Solved.Contains(state.Board))
                        continue;
                    var boardCopy = state.Board.Copy();
                    Solved.Add(boardCopy);
                    yield return boardCopy;
                }
                continue;
            }

            var currentPoint = state.AllPoints[state.PointIndex];

            if (state.Board[currentPoint] is not null)
            {
                // Advance the point index since current is filled.
                var nextState = new SolverState(
                    state.Board,
                    state.PointIndex + 1,
                    state.RemainingBricks,
                    state.Steps,
                    state.Stopwatch,
                    state.BoardPermutations,
                    state.AllPoints
                );
                stack.Push(nextState);
                continue;
            }

            var permutations = state.BoardPermutations[currentPoint.Y, currentPoint.X]
                .BrickPermutations
                .Where(bp => state.RemainingBricks.Contains(bp.OriginalBrick));

            foreach (var brickPermutation in permutations)
            {
                foreach (var variant in brickPermutation.Variants)
                {
                    if (state.Board.IsPossiblePlace(variant) is false)
                        continue;

                    // Clone board and update remaining bricks for the new branch.
                    var newBoard = state.Board.Copy();
                    newBoard.UnsafePlace(variant);

                    var placed = state.RemainingBricks.First(b => b.IsSameShape(variant));
                    var newRemaining = new List<Brick>(state.RemainingBricks);
                    newRemaining.Remove(placed);

                    var newState = new SolverState(
                        newBoard,
                        state.PointIndex + 1,
                        newRemaining,
                        state.Steps,
                        state.Stopwatch,
                        state.BoardPermutations,
                        state.AllPoints
                    );
                    stack.Push(newState);
                }
            }
        }
    }

    // ...possibly other existing code...
}