using System.Collections.Generic;

namespace PuzzleSolver.Web.Models
{
    public class PresetViewModel
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public Dictionary<string, int> Bricks { get; set; }
    }
} 