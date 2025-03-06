namespace PuzzleSolver.Core.Abstract;

public interface ITetrisPuzzleSolver
{
    SolveResult Solve(SolveArguments solveArguments);
}
