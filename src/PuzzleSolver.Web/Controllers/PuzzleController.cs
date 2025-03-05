using Microsoft.AspNetCore.Mvc;
using PuzzleSolver.Core;
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
        public IActionResult Solve(PuzzleViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("Index", model);
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
                    model.Status = "Ошибка: Необходимо указать хотя бы одну фигуру";
                    return View("Index", model);
                }

                var bricksCellsCount = bricks.Sum(v => v.Points.Length);

                if (bricksCellsCount != model.Width * model.Height)
                {
                    model.Status = $"Ошибка: Недостаток или избыток фигур для поля данного размера. " +
                        $"Фигурки заполнят {bricksCellsCount} клеток. " +
                        $"А доска имеет {model.Width} X {model.Height} = {model.Width * model.Height} клеток.";
                    return View("Index", model);
                }
                
                var result = _puzzleSolver.Solve(board, bricks).ToList();

                if (result.Count == 0)
                {
                    model.Status = "Решений не найдено";
                    return View("Index", model);
                }

                model.Result = result.Select(b => new BoardResult
                {
                    Width = b.Size.X,
                    Height = b.Size.Y,
                    Cells = new string[b.Size.Y, b.Size.X]
                }).ToList();

                // Заполняем цвета в результатах
                for (int i = 0; i < result.Count; i++)
                {
                    var boardResult = model.Result[i];
                    var currentBoard = result[i];
                    var typeIndices = new Dictionary<string, int>();

                    for (int y = 0; y < currentBoard.Size.Y; y++)
                    {
                        for (int x = 0; x < currentBoard.Size.X; x++)
                        {
                            var point = new Point(x, y);
                            var brick = currentBoard[point];
                            if (brick != null)
                            {
                                if (!typeIndices.ContainsKey(brick.Type))
                                    typeIndices[brick.Type] = 0;
                                var index = typeIndices[brick.Type]++;
                                var total = brickTypeCounts[brick.Type];

                                var color = brick.Type switch
                                {
                                    "Ladder" => "#0d6efd",
                                    "Line" => "#198754",
                                    "Roof" => "#dc3545",
                                    "L" => "#ffc107",
                                    _ => "#6c757d"
                                };
                                boardResult.Cells[y, x] = color;
                            }
                        }
                    }
                }

                model.Status = "Успешно решено";
            }
            catch (Exception ex)
            {
                model.Status = $"Ошибка: {ex.Message}";
            }

            return View("Index", model);
        }
    }
} 