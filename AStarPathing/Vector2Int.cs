namespace AStarPathing
{
    public struct Vector2Int
    {
        public int X;
        public int Y;

        public int Id;

        public Vector2Int(int x, int y)
        {
            X = x;
            Y = y;

            var sX = x << 16;
            var sY = y;

            Id = sX | sY;
        }

        public override string ToString()
        {
            return string.Format("[{0},{1}]", X, Y);
        }
    }
}