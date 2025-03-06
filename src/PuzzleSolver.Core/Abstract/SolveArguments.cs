using PuzzleSolver.Core.Primitives;

namespace PuzzleSolver.Core.Abstract;

public class SolveArguments
{
    public Board Board { get; }
    public List<Brick> Pool { get; }

    public SolveArguments(Board board, List<Brick> pool)
    {
        Board = board;
        Pool = pool;
    }
}