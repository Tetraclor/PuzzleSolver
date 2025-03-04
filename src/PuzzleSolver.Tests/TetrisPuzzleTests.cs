using PuzzleSolver.Core;
using PuzzleSolver.Core.Primitives;
using Xunit;

namespace PuzzleSolver.Tests;

public class TetrisPuzzleTests
{
    [Fact]
    public void Shift_ShouldMoveBrickCorrectly()
    {
        // Arrange
        var brick = TetrisPuzzle.BrickRoof.Copy();
        var shift = new Point(2, 1);

        // Act
        var shiftedBrick = TetrisPuzzle.Shift(brick, shift);

        // Assert
        Assert.Equal(4, shiftedBrick.Points.Length);
        Assert.Contains(new Point(2, 1), shiftedBrick.Points);
        Assert.Contains(new Point(3, 1), shiftedBrick.Points);
        Assert.Contains(new Point(4, 1), shiftedBrick.Points);
        Assert.Contains(new Point(3, 2), shiftedBrick.Points);
    }

    [Fact]
    public void Rotate_ShouldRotateBrickCorrectly()
    {
        // Arrange
        var brick = TetrisPuzzle.BrickRoof.Copy();
        var center = new Point(1, 1);
        var angle = 90;

        // Act
        var rotatedBrick = TetrisPuzzle.Rotate(brick, center, angle);

        // Assert
        Assert.Equal(4, rotatedBrick.Points.Length);
        Assert.Contains(new Point(2, 0), rotatedBrick.Points);
        Assert.Contains(new Point(2, 1), rotatedBrick.Points);
        Assert.Contains(new Point(2, 2), rotatedBrick.Points);
        Assert.Contains(new Point(1, 1), rotatedBrick.Points);
    }

    [Fact]
    public void Permutations_ShouldGenerateAllPossibleVariants()
    {
        // Arrange
        var brick = TetrisPuzzle.BrickRoof;

        // Act
        var permutations = TetrisPuzzle.Permutations(brick).ToList();

        // Assert
        Assert.Equal(16, permutations.Count); // 4 точки * 4 поворота
    }

    [Fact]
    public void GetMinPoint_ShouldReturnCorrectMinPoint()
    {
        // Arrange
        var brick = TetrisPuzzle.BrickRoof;

        // Act
        var minPoint = TetrisPuzzle.GetMinPoint(brick);

        // Assert
        Assert.Equal(new Point(0, 0), minPoint);
    }

    [Fact]
    public void GetMaxPoint_ShouldReturnCorrectMaxPoint()
    {
        // Arrange
        var brick = TetrisPuzzle.BrickRoof;

        // Act
        var maxPoint = TetrisPuzzle.GetMaxPoint(brick);

        // Assert
        Assert.Equal(new Point(2, 1), maxPoint);
    }

    [Fact]
    public void PlacedOneFromPool_ShouldPlaceBrickIfPossible()
    {
        // Arrange
        var board = new Board(new Point(4, 4));
        var bricks = new List<Brick> { TetrisPuzzle.BrickRoof };
        var point = new Point(0, 0);

        // Act
        var result = TetrisPuzzle.PlacedOneFromPool(board, point, bricks);

        // Assert
        Assert.True(result);
        Assert.NotNull(board[point]);
    }

    [Fact]
    public void PlacedOneFromPool_ShouldNotPlaceBrickIfImpossible()
    {
        // Arrange
        var board = new Board(new Point(4, 4));
        var bricks = new List<Brick> { TetrisPuzzle.BrickRoof };
        var point = new Point(3, 3);

        // Act
        var result = TetrisPuzzle.PlacedOneFromPool(board, point, bricks);

        // Assert
        Assert.False(result);
        Assert.Null(board[point]);
    }

    [Fact]
    public void CreateBrickFromString_ShouldCreateCorrectBrick()
    {
        // Arrange
        var brickString = @"
 *
***
";

        // Act
        var brick = TetrisPuzzle.CreateBrickFromString(brickString);

        // Assert
        Assert.Equal(4, brick.Points.Length);
        Assert.Contains(new Point(1, 0), brick.Points);
        Assert.Contains(new Point(0, 1), brick.Points);
        Assert.Contains(new Point(1, 1), brick.Points);
        Assert.Contains(new Point(2, 1), brick.Points);
    }

    [Fact]
    public void CreateBrickFromString_ShouldHandleEmptyLines()
    {
        // Arrange
        var brickString = @"

 *
***

";

        // Act
        var brick = TetrisPuzzle.CreateBrickFromString(brickString);

        // Assert
        Assert.Equal(4, brick.Points.Length);
        Assert.Contains(new Point(1, 0), brick.Points);
        Assert.Contains(new Point(0, 1), brick.Points);
        Assert.Contains(new Point(1, 1), brick.Points);
        Assert.Contains(new Point(2, 1), brick.Points);
    }

    [Fact]
    public void CreateBrickFromString_ShouldHandleDifferentCharacters()
    {
        // Arrange
        var brickString = @"
 #
###
";

        // Act
        var brick = TetrisPuzzle.CreateBrickFromString(brickString, '#');

        // Assert
        Assert.Equal(4, brick.Points.Length);
        Assert.Contains(new Point(1, 0), brick.Points);
        Assert.Contains(new Point(0, 1), brick.Points);
        Assert.Contains(new Point(1, 1), brick.Points);
        Assert.Contains(new Point(2, 1), brick.Points);
    }

    [Fact]
    public void IsSameShape_ShouldReturnTrueForSameShapeWithDifferentRotation()
    {
        // Arrange
        var brick1 = TetrisPuzzle.CreateBrickFromString(@"
 *
***
");
        var brick2 = TetrisPuzzle.Rotate(brick1.Copy(), new Point(1, 1), 90);

        // Act
        var result = brick1.IsSameShape(brick2);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void IsSameShape_ShouldReturnTrueForSameShapeWithDifferentPosition()
    {
        // Arrange
        var brick1 = TetrisPuzzle.CreateBrickFromString(@"
 *
***
");
        var brick2 = TetrisPuzzle.Shift(brick1.Copy(), new Point(2, 3));

        // Act
        var result = brick1.IsSameShape(brick2);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void IsSameShape_ShouldReturnFalseForDifferentShapes()
    {
        // Arrange
        var brick1 = TetrisPuzzle.BrickRoof;
        var brick2 = TetrisPuzzle.BrickLine;

        // Act
        var result = brick1.IsSameShape(brick2);

        // Assert
        Assert.False(result);
    }
} 