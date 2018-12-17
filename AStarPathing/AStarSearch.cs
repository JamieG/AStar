using System;
using System.Collections.Generic;
using System.Linq;

namespace AStarPathing
{
    public class AStarSearch
    {
        public NodePriorityQueue Open { get; set; }
        public IDictionary<int, Node> Closed { get; set; }
        public Dictionary<int, Node> Parent { get; set; }
        public IDictionary<int, double> FHistory { get; set; }
        public IDictionary<int, double> GHistory { get; set; }
        public IGrid Grid { get; set; }

        public AStarSearch(IGrid grid)
        {
            Grid = grid;

            Open = new NodePriorityQueue(Grid.Width * Grid.Height);
            Closed = new Dictionary<int, Node>(Grid.Width * Grid.Height);
            Parent = new Dictionary<int, Node>(Grid.Width * Grid.Height);
            FHistory = new Dictionary<int, double>(Grid.Width * Grid.Height);
            GHistory = new Dictionary<int, double>(Grid.Width * Grid.Height);
        }

        public Node Start { get; set; }
        public Node Goal { get; set; }

        public double Heuristic(Node node)
        {
            var dX = Math.Abs(node.Location.X - Goal.Location.X) * PathingConstants.HeuristicBias;
            var dY = Math.Abs(node.Location.Y - Goal.Location.Y) * PathingConstants.HeuristicBias;
            
            // Octile distance
            return PathingConstants.CardinalCost * (dX + dY)
                   + (PathingConstants.DiagonalCost - 2 * PathingConstants.CardinalCost)
                   * Math.Min(dX, dY);
        }

        public IList<Node> Find(Node start, Node goal)
        {
            Start = start;
            Goal = goal;

            Parent.Add(start.Location.Id, start);
            Open.Enqueue(Start, Heuristic(Start));

            while (Open.Count > 0)
            {
                Node node = Open.Dequeue();

                if (node.Location.Id == Goal.Location.Id)
                    return BuildPath();

                Closed.AddUpdate(node.Location.Id, node);

                for (int i = 0; i < PathingConstants.Directions.Length; i++)
                {
                    var direction = PathingConstants.Directions[i];

                    var proposed = new Vector2Int(
                        node.Location.X + direction.X,
                        node.Location.Y + direction.Y
                    );
                    
                    if (!Grid.InBounds(proposed))
                        continue;

                    var neighbour = Grid[proposed.X, proposed.Y];

                    if (neighbour.Blocked)
                        continue;

                    if (!Closed.ContainsKey(neighbour.Location.Id))
                    {
                        if (!Open.Contains(neighbour))
                        {
                            GHistory.AddUpdate(neighbour.Location.Id, double.PositiveInfinity);
                            Parent.TryRemove(neighbour.Location.Id);
                        }

                        double gOld = GHistory.TryGetValue(neighbour.Location.Id);

                        // Compute Cost
                        double g = GHistory.TryGetValue(node.Location.Id);
                        if (g + direction.Cost < gOld)
                        {
                            Parent.AddUpdate(neighbour.Location.Id, node);
                            GHistory.AddUpdate(neighbour.Location.Id, g + direction.Cost);
                        }

                        // Compute Cost End
                        double gNew = GHistory.TryGetValue(neighbour.Location.Id);
                        if (gNew < gOld)
                        {
                            if (Open.Contains(neighbour))
                                Open.UpdatePriority(neighbour, gNew + Heuristic(neighbour));
                            else
                                Open.Enqueue(neighbour, gNew + Heuristic(neighbour));
                        }
                    }
                }
            }

            return new List<Node>();
        }

       

        //public TValue TryGetValue<TKey, TValue>(IDictionary<TKey, TValue> dictionary, TKey key)
        //{
        //    TValue result;
        //    if (dictionary.TryGetValue(key, out result))
        //        return result;
        //    return default(TValue);
        //}

        public IList<Node> BuildPath()
        {
            Node current = Goal;
            var path = new Stack<Node>();
            path.Push(current);
            do
            {
                path.Push((current = Parent[current.Location.Id]));
            } while (current.Location.Id != Start.Location.Id);
            return path.ToList();
        }
    }

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