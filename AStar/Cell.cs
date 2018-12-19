namespace AStar
{
    public class Cell : FastPriorityQueueNode
    {
        public Vector2Int Location { get; }
        public int Id { get; }
        public bool Blocked { get; set; }

        public Cell(Vector2Int location, int id)
        {
            Location = location;
            Id = id;
        }

        public override string ToString() => $"[{Location.X},{Location.Y}]";
    }
}