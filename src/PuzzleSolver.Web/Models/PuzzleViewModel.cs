using System.ComponentModel.DataAnnotations;
using PuzzleSolver.Core.Primitives;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace PuzzleSolver.Web.Models
{
    public static class BrickColors
    {
        public static string GetColor(string type) => type switch
        {
            "Ladder" => "#ff0000",    // ярко-красный (зигзаг)
            "Line" => "#333333",      // светло-черный (длинная линия)
            "Roof" => "#ffd700",      // желтый (T-образная)
            "L" => "#00bfff",         // голубой (L-образная)
            "Square" => "#228b22",    // зеленый (квадрат)
            "Small" => "#d3d3d3",     // светло-серый (маленькая линия)
            "Hook" => "#800080",      // фиолетовый (крюк)
            "Crown" => "#ffa500",     // оранжевый (корона)
            _ => "#6c757d"
        };
    }

    public class BrickInput
    {
        public string Type { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public string Shape { get; set; } = string.Empty;
        public int Count { get; set; }

        public string GetColor() => BrickColors.GetColor(Type);
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

        [ModelBinder(BinderType = typeof(ArrayModelBinder<BrickInput>))]
        public List<BrickInput> Bricks { get; set; } = new()
        {
            new BrickInput { Type = "Ladder", DisplayName = "Зигзаг", Shape = "**\n **", Count = 0 },
            new BrickInput { Type = "Line", DisplayName = "Линия", Shape = "****", Count = 0 },
            new BrickInput { Type = "Roof", DisplayName = "T-образная", Shape = " *\n***", Count = 0 },
            new BrickInput { Type = "L", DisplayName = "L-фигура", Shape = "*\n***", Count = 0 },
            new BrickInput { Type = "Square", DisplayName = "Квадрат", Shape = "**\n**", Count = 0 },
            new BrickInput { Type = "Small", DisplayName = "Малая линия", Shape = "**", Count = 0 },
            new BrickInput { Type = "Hook", DisplayName = "Крюк", Shape = "*\n**", Count = 0 },
            new BrickInput { Type = "Crown", DisplayName = "Корона", Shape = "* *\n***", Count = 0 }
        };

        [Display(Name = "Результат")]
        public List<BoardResult> Result { get; set; } = new();

        [Display(Name = "Статус")]
        public string Status { get; set; } = string.Empty;
    }
}
