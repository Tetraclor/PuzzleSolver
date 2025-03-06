using Microsoft.AspNetCore.Mvc;
using PuzzleSolver.Core;
using PuzzleSolver.Core.Abstract;
using PuzzleSolver.Core.Primitives;
using PuzzleSolver.Web.Models;
using System.Text.Json;

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

        [HttpGet]
        public async Task SolveStream([FromQuery] PuzzleViewModel model)
        {
            Response.Headers.Add("Content-Type", "text/event-stream");
            Response.Headers.Add("Cache-Control", "no-cache");
            Response.Headers.Add("Connection", "keep-alive");

            try
            {
                if (model == null)
                {
                    await SendError("Ошибка: Неверные входные данные");
                    return;
                }

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
                    await SendError("Ошибка: Необходимо указать хотя бы одну фигуру");
                    return;
                }

                var bricksCellsCount = bricks.Sum(v => v.Points.Length);

                if (bricksCellsCount != model.Width * model.Height)
                {
                    await SendError($"Ошибка: Недостаток или избыток фигур для поля данного размера. " +
                        $"Фигурки заполнят {bricksCellsCount} клеток. " +
                        $"А доска имеет {model.Width} X {model.Height} = {model.Width * model.Height} клеток.");
                    return;
                }

                var result = _puzzleSolver.Solve(new SolveArguments(board, bricks));
                var solutionCount = 0;

                foreach (var b in result.Boards)
                {
                    if (solutionCount >= 50) break;

                    var boardResult = new BoardResult
                    {
                        Width = b.Size.X,
                        Height = b.Size.Y,
                        Cells = new List<CellInfo>()
                    };

                    var processedCells = new HashSet<string>();
                    var brickId = 0;

                    // Проходим по всем ячейкам доски
                    for (int y = 0; y < b.Size.Y; y++)
                    {
                        for (int x = 0; x < b.Size.X; x++)
                        {
                            var point = new Point(x, y);
                            var brick = b[point];
                            if (brick != null && !processedCells.Contains($"{x},{y}"))
                            {
                                brickId++;
                                var color = BrickColors.GetColor(brick.Type);
                                FillBrickCells(b, point, brick, brickId, color, processedCells, boardResult);
                            }
                        }
                    }

                    await SendSolution(boardResult);
                    solutionCount++;
                }

                if (solutionCount == 0)
                {
                    await SendError("Решений не найдено");
                }
                else
                {
                    await SendComplete($"Найдено решений: {solutionCount}");
                }
            }
            catch (Exception ex)
            {
                await SendError($"Ошибка: {ex.Message}");
            }
        }

        private void FillBrickCells(Board board, Point point, Brick brick, int brickId, string color, 
            HashSet<string> processedCells, BoardResult boardResult)
        {
            var key = $"{point.X},{point.Y}";
            if (processedCells.Contains(key)) return;
            if (board[point] != brick) return;

            processedCells.Add(key);
            boardResult.Cells.Add(new CellInfo
            {
                X = point.X,
                Y = point.Y,
                Color = color,
                BrickId = brickId
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
                if (neighbor.X >= 0 && neighbor.X < board.Size.X &&
                    neighbor.Y >= 0 && neighbor.Y < board.Size.Y)
                {
                    FillBrickCells(board, neighbor, brick, brickId, color, processedCells, boardResult);
                }
            }
        }

        private async Task SendSolution(BoardResult solution)
        {
            var json = JsonSerializer.Serialize(new { type = "solution", data = solution });
            await Response.WriteAsync($"data: {json}\n\n");
            await Response.Body.FlushAsync();
        }

        private async Task SendError(string message)
        {
            var json = JsonSerializer.Serialize(new { type = "error", message });
            await Response.WriteAsync($"data: {json}\n\n");
            await Response.Body.FlushAsync();
        }

        private async Task SendComplete(string message)
        {
            var json = JsonSerializer.Serialize(new { type = "complete", message });
            await Response.WriteAsync($"data: {json}\n\n");
            await Response.Body.FlushAsync();
        }
    }
}
