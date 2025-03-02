using PuzzleSolver.Core.Primitives;

namespace PuzzleSolver.Core;

public interface ITetrisPuzzleSolver
{
    IEnumerable<Board> Solve(Board board, List<Brick> pool);
}
