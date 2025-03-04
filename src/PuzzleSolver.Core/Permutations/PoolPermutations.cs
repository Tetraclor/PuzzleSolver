using PuzzleSolver.Core.Primitives;

namespace PuzzleSolver.Core.Permutations;

public class PoolPermutations
{
    public List<BrickPermutation> BrickPermutations;

    public PoolPermutations(List<Brick> pool, Point pointOnBoard)
    {
        BrickPermutations = [];

        foreach (var brick in pool)
        {
            var brickPermutation = BrickPermutations.FirstOrDefault(x => x.OriginalBrick.Equals(brick));

            if (brickPermutation != null)
            {
                brickPermutation.CopyCount++;
            }
            else
            {
                var variants = TetrisPuzzle.Permutations(brick).ToList();

                foreach (var variant in variants)
                {
                    TetrisPuzzle.Shift(variant, pointOnBoard);
                }

                BrickPermutations.Add(new BrickPermutation()
                {
                    OriginalBrick = brick,
                    PointOnBoard = pointOnBoard,
                    Variants = variants,
                    CopyCount = 1
                });
            }
        }
    }

    public void RemoveBrick(Brick brick)
    {
        var brickPermutation = BrickPermutations.FirstOrDefault(x => x.OriginalBrick.Equals(brick));

        if (brickPermutation is null || brickPermutation.CopyCount == 0)
        {
            return;
        }

        brickPermutation.CopyCount--;

        if (brickPermutation.CopyCount == 0)
        {
            BrickPermutations.Remove(brickPermutation);
        }
    }

    public void RemoveVariant(Brick original, Brick variant)
    {
        var brickPermutation = BrickPermutations.FirstOrDefault(x => x.OriginalBrick.Equals(original));

        if (brickPermutation is null)
        {
            return;
        }

        brickPermutation.Variants.Remove(variant);
    }
} 