using PuzzleSolver.Core.Primitives;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;

namespace PuzzleSolver.Core;

internal class Program
{
    static void Main(string[] args)
    {

        var stopwatch = Stopwatch.StartNew();

        Solve();

        stopwatch.Stop();

        Console.WriteLine($"Времени затрачено на решение: {stopwatch.Elapsed}");

        //var permutations = TetrisPuzzle
        //    .Permutations(TetrisPuzzle.BrickRoof)
        //    .Distinct();

        //foreach (var variant in permutations)
        //{
        //    var norm = TetrisPuzzle.Shift(variant, new Point(2, 2));

        //    var board = new Board(new Point(8, 8));

        //    board.TryPlace(norm);

        //    PrintBoard(board);
        //}
    }

    static void Solve()
    {
        var pool = new List<Brick>();

        var size = new Point(5, 5);
        var sourceBoard = new Board(size);

        var permutations = TetrisPuzzle
            .Permutations(TetrisPuzzle.BrickRoof)
            .ToArray();
        
        var allPoints = sourceBoard.GetAllPoints().ToArray();
        var solved = new List<Board>();

        var boardVariants = new List<(Board, int)>();
        var boardVariantsWithoutIndex = new List<Board>();

        ulong iterations = 0;

        Req(sourceBoard, 0);

        //foreach(var bo in boardVariants)
        //{
        //    PrintBoard(bo.Item1);
        //}

        var solvedHashSet = solved.Distinct().ToList();


        Console.WriteLine($"Все заполенные варианты: {++iterations}");
        Console.WriteLine($"Все возможные варианты: {boardVariants.Count}");

        void Req(Board board, int pointIndex)
        {
            if (pointIndex == allPoints.Length)
            {
                iterations++;
                if (iterations % 100 == 0)
                {
                    Console.WriteLine(iterations);
                    Console.WriteLine(boardVariants.Count);
                }
                if (board.IsFilled())
                {
                    if (!solved.Contains(board))
                    {
                        Console.WriteLine("Новое решение найдено:");
                        PrintBoard(board);
                        solved.Add(board);
                    }
                }
                return;
            }

            var currentPoint = allPoints[pointIndex];

            if (board[currentPoint] is not null)
            {
                return;
            }

            //if (boardVariants.Contains((board, pointIndex)))
            //{
            //    return;
            //}

            //boardVariants.Add((board, pointIndex));

            foreach (var permut in permutations)
            {
                var copyBrick = TetrisPuzzle.Shift(permut.Copy(), currentPoint);
                var copyBoard = board.Copy();
                var isPlaced = copyBoard.TryPlace(copyBrick);

                if (isPlaced)
                {
                    Req(copyBoard, pointIndex + 1);
                }   
            }

            // Вариант, ничего не вставлять. 
            Req(board.Copy(), pointIndex + 1);
        }
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
