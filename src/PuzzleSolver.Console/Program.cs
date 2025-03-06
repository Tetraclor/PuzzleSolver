﻿using PuzzleSolver.Core.Abstract;
using PuzzleSolver.Core.Primitives;
using PuzzleSolver.Core.Solvers;
using System.Diagnostics;

namespace PuzzleSolver.Core;

internal class Program
{
    static void Main(string[] args)
    {
        var board = new Board(new Point(12, 12));

        var pool = new List<Brick>() { };

        var cellCount = board.Size.X * board.Size.Y;

        for (var i = 0; i < cellCount / TetrisPuzzle.BrickRoof.Points.Length; i++)
        {
            pool.Add(TetrisPuzzle.BrickRoof);
        }

       // pool = [TetrisPuzzle.BrickRoof, TetrisPuzzle.BrickRoof, TetrisPuzzle.BrickLine, TetrisPuzzle.BrickL];

        var tetrisSolver6 = new TetrisPuzzleSolver8();

        var solvers = new List<ITetrisPuzzleSolver>() 
        {
            tetrisSolver6,
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
                Console.WriteLine($"======= {board.Size.X} X {board.Size.Y} =======");

                var result = solver.Solve(new SolveArguments(board, pool));

                var list = result.Boards.ToList();

                Console.WriteLine($"Решений найдено: {list.Count}");

                //foreach (var solve in list)
                //{
                //    Console.WriteLine(solve);
                //}
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
