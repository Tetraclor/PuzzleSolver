using PuzzleSolver.Core.Primitives;

namespace PuzzleSolver.Core.Abstract;

public class SolveResult
{
    public IEnumerable<Board> Boards { get; }
    public int Steps { get; }

    public SolveResult(IEnumerable<Board> boards, int steps)
    {
        Boards = boards;
        Steps = steps;
    }
}
