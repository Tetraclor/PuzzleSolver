using PuzzleSolver.Core.Primitives;

namespace PuzzleSolver.Core;

public class TetrisPuzzleSolver2 : ITetrisPuzzleSolver
{
    

    public IEnumerable<Board> Solve(Board board, List<Brick> pool)
    {
        var permutations = pool
            .SelectMany(brick =>
            {
                return TetrisPuzzle
                    .Permutations(brick)
                    .ToArray();
            })
            .Distinct()
            .ToArray();

        var allPoints = board.GetAllPoints().ToArray();
        var solved = new List<Board>();

        ulong iterations = 0;
        ulong steps = 0;

        Req(board, 0);

        Console.WriteLine($"Все заполенные варианты: {++iterations}");
        Console.WriteLine($"Шагов сделано: {steps}");

        return solved;

        void Req(Board board, int pointIndex)
        {
            steps++;

            if (pointIndex == allPoints.Length)
            {
                iterations++;
                if (iterations % 100_000 == 0)
                {
                    Console.WriteLine(steps);
                }
                if (board.IsFilled2())
                {
                    if (!solved.Contains(board))
                    {
                        Console.WriteLine("Новое решение найдено.");
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

            foreach (var permut in permutations)
            {
                var copyPermut = permut.Copy();
                var shiftBrick = TetrisPuzzle.Shift(copyPermut, currentPoint);

                if (board.IsPossiblePlace(shiftBrick) is false)
                {
                    continue;
                }

                var copyBoard = board.Copy();

                copyBoard.UnsafePlace(shiftBrick);

                Req(copyBoard, pointIndex + 1);
            }

            // Вариант, ничего не вставлять. 
            Req(board.Copy(), pointIndex + 1);
        }
    }


    public void PermutationMask(List<Brick> permutations)
    {
        var permutationsMask = new int[10, 10];

        foreach (var permut in permutations)
        {
            var copy = permut.Copy();
            var shift = TetrisPuzzle.Shift(copy, new Point(5, 5));

            foreach (var point in shift.Points)
            {
                permutationsMask[point.Y, point.X]++;
            }
        }

        // Вывод массива в консоль
        Console.WriteLine("Массив permutationsMask:");
        for (int i = 0; i < permutationsMask.GetLength(0); i++)
        {
            for (int j = 0; j < permutationsMask.GetLength(1); j++)
            {
                // Форматирование вывода: каждое число занимает 3 символа (включая пробелы)
                Console.Write(permutationsMask[i, j].ToString().PadLeft(3) + " ");
            }
            Console.WriteLine(); // Переход на новую строку после каждой строки массива
        }
    }
}