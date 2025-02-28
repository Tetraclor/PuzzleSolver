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
        for(var y = 0; y < 5; y++)
        {
            for(var x = 0; x < 5; x++)
            {
                if (brick.Points.Any(v => v.X == x && v.y == y))
                {
                    Console.Write('#');
                }
                else
                {
                    Console.Write(' ');
                }
            }

            Console.WriteLine();
        }
    }
}
