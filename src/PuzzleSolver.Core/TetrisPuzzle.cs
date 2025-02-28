using PuzzleSolver.Core.Primitives;

namespace PuzzleSolver.Core;

public class TetrisPuzzle
{
    // **
    //  **
    public static Brick BrickLadder = new ()
    {
        Points =
        [
            new Point(0, 0),
            new Point(1, 0),
            new Point(1, 1),
            new Point(2, 1),
        ]
    };

    //
    // 
    public static Brick BrickLine = new()
    {
        Points =
        [
            new Point(0, 0),
            new Point(1, 0),
            new Point(2, 0),
            new Point(3, 0),
        ]
    };

    // TODO добавить другие фигурки.

    public List<Brick> Bricks = new ()
    {
        BrickLadder,
        BrickLine,
    };
}