using PuzzleSolver.Core.Primitives;

namespace PuzzleSolver.Core;

internal class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Вывод всех фигурок");

        var tetrisPuzzle = new TetrisPuzzle();

        foreach (var brick in tetrisPuzzle.Bricks) 
        {
            Console.WriteLine();
            PrintBrick(brick);
        }
    }

    static void PrintBrick(Brick brick)
    {
        // TODO сделать отображение
    }
}
