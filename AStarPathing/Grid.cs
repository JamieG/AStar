using AStar;

namespace AStarPathing
{
    public class Grid : IGrid
    {
        private readonly Cell[,] _cells;

        public int Width { get; set; }
        public int Height { get; set; }

        public Cell this[int x, int y] => _cells[x, y];

        public Cell this[Vector2Int location] => _cells[location.X, location.Y];

        public Grid(int width, int height)
        {
            Width = width;
            Height = height;

            _cells = new Cell[width, height];
            for (var x = 0; x <= width - 1; x++)
            for (var y = 0; y <= height - 1; y++)
                _cells[x, y] = new Cell(new Vector2Int(x, y), x * width + y);
        }

        public int GetNodeId(Vector2Int location) => location.X * Width + location.Y;
    }
}