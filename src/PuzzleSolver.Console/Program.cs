using PuzzleSolver.Core.Primitives;
using System.Diagnostics;

namespace PuzzleSolver.Core;

internal class Program
{
    static void Main(string[] args)
    {
        var board = new Board(new Point(7, 6));
        var pool = new List<Brick>() { TetrisPuzzle.BrickRoof };

        var tetrisSolver5 = new TetrisPuzzleSolver5();

        var solvers = new List<ITetrisPuzzleSolver>() 
        {
            tetrisSolver5,
          //  new TetrisPuzzleSolver(),
        };

        foreach(var solver in solvers)
        {
            if (TetrisPuzzle.CheckSolver(solver) is false)
            {
                Console.WriteLine("Решатель неправильно работает.");
            }

            Time(() =>
            {
                var result = solver.Solve(board, pool).ToList();

                foreach (var solve in result)
                {
                    Console.WriteLine(solve);
                }
            });


        }

        //foreach (var solve in tetrisSolver2.All)
        //{
        //    PrintBoard(solve);
        //}
    }


    static void Time(Action action)
    {
        var stopwatch = Stopwatch.StartNew();

        action();

        stopwatch.Stop();

        Console.WriteLine($"Времени затрачено на решение: {stopwatch.Elapsed}");
    }

    void Check() 
    {
        var permutations = TetrisPuzzle
            .Permutations(TetrisPuzzle.BrickRoof)
            .ToArray();


        foreach (var permut in permutations)
        {
            var copy = permut.Copy();

            TetrisPuzzle.Shift(copy, new Point(0, 0));

            var board1 = new Board(new Point(5, 5));

            board1.TryPlace(copy);

            Console.WriteLine(board1);
        }

        var list = new List<Brick>();

        foreach (var permut in permutations)
        {
            if (list.Contains(permut))
            {
                var index = list.IndexOf(permut);

                var copy = list[index];
                continue;
            }
            list.Add(permut);
        }
        //   return;
    }
}
