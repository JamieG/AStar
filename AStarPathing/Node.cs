namespace AStarPathing
{
    public class Node : FastPriorityQueueNode
    {
        public Vector2Int Location { get; private set; }

        public bool Blocked { get; set; }

        public Node(int x, int y)
        {
            Location = new Vector2Int(x, y);
        }

        public override string ToString()
        {
            return string.Format("[{0},{1}]", Location.X, Location.Y);
        }
    }
}