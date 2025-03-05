using System.ComponentModel.DataAnnotations;
using PuzzleSolver.Core.Primitives;
using System.Collections.Generic;

namespace PuzzleSolver.Web.Models
{
    public class BrickInput
    {
        public string Type { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public string Shape { get; set; } = string.Empty;
        public int Count { get; set; }

        public string GetColor() => Type switch
        {
            "Ladder" => "#0d6efd",
            "Line" => "#198754",
            "Roof" => "#dc3545",
            "L" => "#ffc107",
            _ => "#6c757d"
        };
    }

    public class PuzzleViewModel
    {
        [Required(ErrorMessage = "Пожалуйста, введите ширину поля")]
        [Range(1, 20, ErrorMessage = "Ширина поля должна быть от 1 до 20")]
        [Display(Name = "Ширина поля")]
        public int Width { get; set; } = 4;

        [Required(ErrorMessage = "Пожалуйста, введите высоту поля")]
        [Range(1, 20, ErrorMessage = "Высота поля должна быть от 1 до 20")]
        [Display(Name = "Высота поля")]
        public int Height { get; set; } = 4;

        public List<BrickInput> Bricks { get; set; } = new()
        {
            new BrickInput { Type = "Ladder", DisplayName = "Лестница", Shape = "**\n **", Count = 0 },
            new BrickInput { Type = "Line", DisplayName = "Линия", Shape = "****", Count = 2 },
            new BrickInput { Type = "Roof", DisplayName = "Крыша", Shape = "***\n *", Count = 0 },
            new BrickInput { Type = "L", DisplayName = "L-фигура", Shape = "*\n***", Count = 2 }
        };

        [Display(Name = "Результат")]
        public List<BoardResult> Result { get; set; } = new();

        [Display(Name = "Статус")]
        public string Status { get; set; } = string.Empty;
    }
}
