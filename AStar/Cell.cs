namespace AStar
{
    public class Cell {

        public int QueueIndex;

        public Vector2Int Location;
        public bool Blocked;

        public double G;
        public double H;
        public double F;

        public Cell Parent;
        public bool Closed;

        public Cell(Vector2Int location)
        {
            Location = location;
        }

        public override string ToString() => $"[{Location.X},{Location.Y}]";
    }
}