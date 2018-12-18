using System.Drawing;

namespace AStarPathing
{
    public class Grid : IGrid
    {
        public static readonly Rectangle[] Obstacles =
        {
            new Rectangle(10, 45, 90, 5),
            new Rectangle(10, 65, 80, 5),
            new Rectangle(45, 65, 5, 20),
            new Rectangle(65, 45, 5, 20),
            new Rectangle(50, 10, 5, 40)
        };

        private readonly int _boundsMaxX;
        private readonly int _boundsMaxY;

        private readonly int _boundsMinX;
        private readonly int _boundsMinY;

        private readonly Node[,] _nodes;

        public Grid(int width, int height)
        {
            Width = width;
            Height = height;

            _boundsMinX = 0;
            _boundsMaxX = width - 1;
            _boundsMinY = 0;
            _boundsMaxY = height - 1;

            _nodes = new Node[width, height];
            for (var x = _boundsMinX; x <= _boundsMaxX; x++)
            for (var y = _boundsMinY; y <= _boundsMaxY; y++)
                _nodes[x, y] = new Node(x, y);
        }

        public int Width { get; set; }
        public int Height { get; set; }

        public Node this[int x, int y]
        {
            get { return _nodes[x, y]; }
        }

        public Node this[Vector2Int location]
        {
            get { return _nodes[location.X, location.Y]; }
        }

        public bool InBounds(Vector2Int proposed)
        {
            return proposed.X >= _boundsMinX && proposed.X <= _boundsMaxX &&
                   proposed.Y >= _boundsMinY && proposed.Y <= _boundsMaxY;
        }

        public bool Collided(Vector2Int proposed)
        {
            for (var i = 0; i < Obstacles.Length; i++)
                if (proposed.X >= Obstacles[i].X && proposed.X <= Obstacles[i].X + Obstacles[i].Width &&
                    proposed.Y >= Obstacles[i].Y && proposed.Y <= Obstacles[i].Y + Obstacles[i].Height)
                    return true;

            return false;
        }
    }
}