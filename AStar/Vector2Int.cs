namespace AStar
{
    public struct Vector2Int
    {
        public int X;
        public int Y;

        public Vector2Int(int x, int y)
        {
            X = x;
            Y = y;
        }

        public override string ToString() => $"[{X},{Y}]";
    }
}