using System.Drawing;

namespace AStarPathing
{
    public class Grid : IGrid
    {
        public int Width { get; set; }
        public int Height { get; set; }

        private readonly Node[,] _nodes;

        public Node this[int x, int y]
        {
            get { return _nodes[x, y]; }
        }

        public Node this[Vector2Int location]
        {
            get { return _nodes[location.X, location.Y]; }
        }

        public bool Collided(Vector2Int proposed)
        {
            for (int i = 0; i < Obstacles.Length; i++)
            {
                if (proposed.X >= Obstacles[i].X && proposed.X <= Obstacles[i].X + Obstacles[i].Width &&
                    proposed.Y >= Obstacles[i].Y && proposed.Y <= Obstacles[i].Y + Obstacles[i].Height)
                {
                    return true;
                }
            }

            return false;
        }

        private readonly int _boundsMinX;
        private readonly int _boundsMaxX;
        private readonly int _boundsMinY;
        private readonly int _boundsMaxY;

        public bool InBounds(Vector2Int proposed)
        {
            return proposed.X >= _boundsMinX && proposed.X <= _boundsMaxX &&
                   proposed.Y >= _boundsMinY && proposed.Y <= _boundsMaxY;
        }

        public Grid(short width, short height)
        {
            Width = width;
            Height = height;

            _boundsMinX = 0;
            _boundsMaxX = width-1;
            _boundsMinY = 0;
            _boundsMaxY = height-1;

            _nodes = new Node[width, height];
            for (int x = _boundsMinX; x <= _boundsMaxX; x++)
                for (int y = _boundsMinY; y <= _boundsMaxY; y++)
                    _nodes[x,y] = new Node(x, y) {Blocked = Collided(new Vector2Int(x, y))};

            for (int i = 0; i < PathingConstants.Directions.Length; i++)
                _proposals[i] = new ProposedStep(PathingConstants.Directions[i]);
        }

        public static readonly Rectangle[] Obstacles = new[]
        {
            new Rectangle(10, 45, 90, 5),
            new Rectangle(0, 65, 80, 5),
            new Rectangle(45, 65, 5, 20),
            //new Rectangle(65, 45, 5, 20),
            new Rectangle(50, 10, 5, 40),
        };


        private readonly ProposedStep[] _proposals = new ProposedStep[PathingConstants.Directions.Length];
        
        public ProposedStep[] Neighbours(Node node)
        {
            for (int i = 0; i < PathingConstants.Directions.Length; i++)
            {
                var proposed = new Vector2Int(node.Location.X + PathingConstants.Directions[i].X, node.Location.Y + PathingConstants.Directions[i].Y);
                if (InBounds(proposed))
                {
                    var neighbour = _nodes[proposed.X, proposed.Y];
                    if (!neighbour.Blocked)
                    {
                        _proposals[i].Node = neighbour;
                        continue;
                    }
                }

                _proposals[i].Node = null;
            }

            return _proposals;
        }

       
    }
}