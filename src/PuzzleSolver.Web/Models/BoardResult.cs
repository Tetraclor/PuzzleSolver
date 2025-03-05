using System.Collections.Generic;

namespace PuzzleSolver.Web.Models
{
    public class BoardResult
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public List<CellInfo> Cells { get; set; } = new List<CellInfo>();
        public string[,] CellsGrid { get; set; }
    }

    public class CellInfo
    {
        public int X { get; set; }
        public int Y { get; set; }
        public string Color { get; set; }
    }
} 