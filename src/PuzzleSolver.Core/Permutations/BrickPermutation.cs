using PuzzleSolver.Core.Primitives;

namespace PuzzleSolver.Core.Permutations;

public class BrickPermutation
{
    public Brick OriginalBrick;
    public Point PointOnBoard;
    public List<Brick> Variants;
    public int CopyCount;
} 