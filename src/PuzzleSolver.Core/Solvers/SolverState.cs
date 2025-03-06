using PuzzleSolver.Core.Primitives;
using PuzzleSolver.Core.Permutations;
using System.Diagnostics;

namespace PuzzleSolver.Core.Solvers;

public class SolverState
{
    public Board Board { get; set; }
    public int PointIndex { get; set; }
    public List<Brick> RemainingBricks { get; set; }
    public ulong Steps { get; set; }
    public Stopwatch Stopwatch { get; set; }
    public PoolPermutations[,] BoardPermutations { get; set; }
    public Point[] AllPoints { get; set; }

    public SolverState(Board board, int pointIndex, List<Brick> remainingBricks, ulong steps, Stopwatch stopwatch, PoolPermutations[,] boardPermutations, Point[] allPoints)
    {
        Board = board;
        PointIndex = pointIndex;
        RemainingBricks = remainingBricks;
        Steps = steps;
        Stopwatch = stopwatch;
        BoardPermutations = boardPermutations;
        AllPoints = allPoints;
    }

    public SolverState()
    {
    }
}
