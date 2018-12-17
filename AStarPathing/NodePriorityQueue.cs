namespace AStarPathing
{
    public class NodePriorityQueue : FastPriorityQueue<Node>
    {
        public NodePriorityQueue(int maxNodes) : base(maxNodes)
        { }
    }
}