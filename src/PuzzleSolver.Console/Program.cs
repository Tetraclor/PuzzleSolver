﻿using PuzzleSolver.Core.Primitives;
using System.Diagnostics;

namespace PuzzleSolver.Core;

internal class Program
{
    static void Main(string[] args)
    {
        var solver = new TetrisPuzzleSolver();

        if (TetrisPuzzle.CheckSolver(solver) is false)
        {
            Console.WriteLine("Решатель неправильно работает.");
        }

        var board = new Board(new Point(5, 5));
        var pool = new List<Brick>() { TetrisPuzzle.BrickRoof };

        Time(() =>
        {
            var result = solver.Solve(board, pool).ToList();

            foreach (var solve in result)
            {
                PrintBoard(solve);
            }
        });
    }


    static void Time(Action action)
    {
        var stopwatch = Stopwatch.StartNew();

        action();

        stopwatch.Stop();

        Console.WriteLine($"Времени затрачено на решение: {stopwatch.Elapsed}");
    }

    static void PrintBoard(Board board)
    {
        var emptyChar = ' ';
        var horizontalBorderChar = '-';
        var verticalBorderChar = '|';
        var cornerChar = '+';

        var exists = new Dictionary<Brick, char>();
        var currentChar = 'a';

        for (var y = -1; y <= board.Size.Y; y++)
        {
            for (var x = -1; x <= board.Size.X; x++)
            {
                if ((y == -1 || y == board.Size.Y) && (x == -1 || x == board.Size.X))
                {
                    // Углы
                    Console.Write(cornerChar);
                }
                else if (y == -1 || y == board.Size.Y)
                {
                    // Горизонтальные границы
                    Console.Write(horizontalBorderChar);
                }
                else if (x == -1 || x == board.Size.X)
                {
                    // Вертикальные границы
                    Console.Write(verticalBorderChar);
                }
                else if (board.Field[y][x] is not null)
                {
                    var brick = board.Field[y][x];
                    if (!exists.ContainsKey(brick))
                    {
                        exists[brick] = ++currentChar;
                    }
                    Console.Write(exists[brick]);

                }
                else
                {
                    Console.Write(emptyChar);
                }
            }

            Console.WriteLine();
        }
    }
}
