using PuzzleSolver.Core.Primitives;

namespace PuzzleSolver.Core.Permutations;

public class PermutationMaskGenerator
{
    private int[,] permutationsMask; // Маска перестановок (битовые маски)
    private int maxBrickIndex; // Максимальный индекс Brick
    private List<Brick> bricks;

    public PermutationMaskGenerator()
    {
        permutationsMask = new int[10, 10];
        maxBrickIndex = -1; // Инициализируем как -1, так как еще не обработаны Brick
    }

    /// <summary>
    /// Генерирует битовую маску перестановок для заданного списка Brick.
    /// </summary>
    /// <param name="permutations">Список Brick для анализа.</param>
    public void GenerateMask(List<Brick> permutations)
    {
        bricks = permutations;

        // Сбрасываем маску перед новой генерацией
        for (int i = 0; i < 10; i++)
        {
            for (int j = 0; j < 10; j++)
            {
                permutationsMask[i, j] = 0;
            }
        }

        maxBrickIndex = permutations.Count - 1; // Устанавливаем максимальный индекс Brick

        // Проходим по каждому Brick в списке permutations
        for (int brickIndex = 0; brickIndex <= maxBrickIndex; brickIndex++)
        {
            var permut = permutations[brickIndex];
            var copy = permut.Copy();
            var shift = TetrisPuzzle.Shift(copy, new Point(5, 5));

            foreach (var point in shift.Points)
            {
                int x = point.X;
                int y = point.Y;

                // Проверяем, что координаты находятся в пределах маски
                if (x >= 0 && x < 10 && y >= 0 && y < 10)
                {
                    // Устанавливаем бит, соответствующий текущему Brick
                    permutationsMask[y, x] |= 1 << brickIndex;
                }
            }
        }
    }

    /// <summary>
    /// Возвращает список объектов Brick, затрагивающих заданную точку.
    /// </summary>
    /// <param name="x">Координата X точки.</param>
    /// <param name="y">Координата Y точки.</param>
    /// <param name="bricks">Список всех Brick.</param>
    /// <returns>Список объектов Brick.</returns>
    public List<Brick> GetBricksAtPoint(Point point)
    {
        var x = point.X;
        var y = point.Y;
        if (x >= 0 && x < 10 && y >= 0 && y < 10)
        {
            int mask = permutationsMask[y, x];
            var result = new List<Brick>();

            // Проверяем каждый бит в маске
            for (int i = 0; i <= maxBrickIndex; i++)
            {
                if ((mask & 1 << i) != 0 && i < bricks.Count)
                {
                    result.Add(bricks[i]); // Добавляем объект Brick, если соответствующий бит установлен
                }
            }

            return result;
        }
        return []; // Возвращаем пустой список, если точка вне границ
    }
}