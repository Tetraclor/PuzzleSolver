using Microsoft.AspNetCore.Mvc;
using PuzzleSolver.Core;
using PuzzleSolver.Core.Abstract;
using PuzzleSolver.Core.Primitives;
using PuzzleSolver.Web.Models;

namespace PuzzleSolver.Web.Controllers
{
    public class PuzzleController : Controller
    {
        private readonly ITetrisPuzzleSolver _puzzleSolver;

        public PuzzleController(ITetrisPuzzleSolver puzzleSolver)
        {
            _puzzleSolver = puzzleSolver;
        }

        public IActionResult Index()
        {
            return View(new PuzzleViewModel());
        }

        [HttpPost]
        public IActionResult Solve([FromBody] PuzzleViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();
                return Json(new { success = false, status = "Ошибка валидации: " + string.Join(", ", errors) });
            }

            try
            {
                var board = new Board(new Point(model.Width, model.Height));
                var bricks = new List<Brick>();
                var brickTypes = new List<string>();
                var brickTypeCounts = new Dictionary<string, int>();

                // Создаем фигуры на основе введенных количеств
                foreach (var brickInput in model.Bricks)
                {
                    if (brickInput.Count <= 0) continue;

                    var brick = brickInput.Type switch
                    {
                        "Ladder" => TetrisPuzzle.BrickLadder,
                        "Line" => TetrisPuzzle.BrickLine,
                        "Roof" => TetrisPuzzle.BrickRoof,
                        "L" => TetrisPuzzle.BrickL,
                        "Square" => TetrisPuzzle.BrickSquare,
                        "Small" => TetrisPuzzle.BrickSmall,
                        "Hook" => TetrisPuzzle.BrickHook,
                        "Crown" => TetrisPuzzle.BrickCrown,
                        _ => throw new ArgumentException($"Неизвестный тип фигуры: {brickInput.Type}")
                    };

                    for (int i = 0; i < brickInput.Count; i++)
                    {
                        bricks.Add(brick);
                        brickTypes.Add(brickInput.Type);
                        if (!brickTypeCounts.ContainsKey(brickInput.Type))
                            brickTypeCounts[brickInput.Type] = 0;
                        brickTypeCounts[brickInput.Type]++;
                    }
                }

                if (!bricks.Any())
                {
                    return Json(new { success = false, status = "Ошибка: Необходимо указать хотя бы одну фигуру" });
                }

                var bricksCellsCount = bricks.Sum(v => v.Points.Length);

                if (bricksCellsCount != model.Width * model.Height)
                {
                    return Json(new { 
                        success = false, 
                        status = $"Ошибка: Недостаток или избыток фигур для поля данного размера. " +
                            $"Фигурки заполнят {bricksCellsCount} клеток. " +
                            $"А доска имеет {model.Width} X {model.Height} = {model.Width * model.Height} клеток."
                    });
                }
                
                var result = _puzzleSolver.Solve(new SolveArguments(board, bricks));
                var boards = result.Boards.Take(50).ToList();

                if (boards.Count == 0)
                {
                    return Json(new { success = false, status = "Решений не найдено" });
                }

                model.Result = boards.Select(b => new BoardResult
                {
                    Width = b.Size.X,
                    Height = b.Size.Y,
                    Cells = new List<CellInfo>()
                }).ToList();

                // Заполняем цвета в результатах
                for (int i = 0; i < boards.Count; i++)
                {
                    var boardResult = model.Result[i];
                    var currentBoard = boards[i];
                    var brickId = 0;
                    var processedCells = new HashSet<string>();

                    // Функция для рекурсивного заполнения ячеек одной фигуры
                    void FillBrickCells(Point point, Brick brick, int currentBrickId, string color)
                    {
                        var key = $"{point.X},{point.Y}";
                        if (processedCells.Contains(key)) return;
                        if (currentBoard[point] != brick) return;

                        processedCells.Add(key);
                        boardResult.Cells.Add(new CellInfo 
                        { 
                            X = point.X, 
                            Y = point.Y, 
                            Color = color,
                            BrickId = currentBrickId
                        });

                        // Проверяем соседние ячейки
                        var neighbors = new[]
                        {
                            new Point(point.X + 1, point.Y),
                            new Point(point.X - 1, point.Y),
                            new Point(point.X, point.Y + 1),
                            new Point(point.X, point.Y - 1)
                        };

                        foreach (var neighbor in neighbors)
                        {
                            if (neighbor.X >= 0 && neighbor.X < currentBoard.Size.X &&
                                neighbor.Y >= 0 && neighbor.Y < currentBoard.Size.Y)
                            {
                                FillBrickCells(neighbor, brick, currentBrickId, color);
                            }
                        }
                    }

                    // Проходим по всем ячейкам доски
                    for (int y = 0; y < currentBoard.Size.Y; y++)
                    {
                        for (int x = 0; x < currentBoard.Size.X; x++)
                        {
                            var point = new Point(x, y);
                            var brick = currentBoard[point];
                            if (brick != null && !processedCells.Contains($"{x},{y}"))
                            {
                                brickId++;
                                var color = BrickColors.GetColor(brick.Type);
                                FillBrickCells(point, brick, brickId, color);
                            }
                        }
                    }
                }

                return Json(new { success = true, status = "Успешно решено", results = model.Result });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, status = $"Ошибка: {ex.Message}" });
            }
        }
    }
}
