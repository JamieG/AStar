namespace CapitalStaging
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class NodePriorityQueue : FastPriorityQueue<Node>
    {
        public NodePriorityQueue(int maxNodes) : base(maxNodes)
        { }
    }

    public class AStarSearch
    {
        public NodePriorityQueue Open { get; set; }
        public IDictionary<ulong, Node> Closed { get; set; }
        public Dictionary<Node, Node> Parent { get; set; }
        public IDictionary<ulong, double> FHistory { get; set; }
        public IDictionary<ulong, double> GHistory { get; set; }
        public IGrid Grid { get; set; }

        public AStarSearch(IGrid grid)
        {
            Grid = grid;

            Open = new NodePriorityQueue(Grid.Width*Grid.Height);
            Closed = new Dictionary<ulong, Node>();
            Parent = new Dictionary<Node, Node>();
            FHistory = new Dictionary<ulong, double>();
            GHistory = new Dictionary<ulong, double>();
        }

        public Node Start { get; set; }
        public Node Goal { get; set; }
        
        public double Heuristic(Node node)
        {
            return Math.Sqrt(Math.Pow(node.X - Goal.X, 2) + Math.Pow(node.Y - Goal.Y, 2));
        }

        public IList<Node> Find(Node start, Node goal)
        {
            Start = start;
            Goal = goal;

            Parent.Add(start, start);
            Open.Enqueue(Start, Heuristic(Start));

            while (Open.Count > 0)
            {
                Node node = Open.Dequeue();

                if (node.Id == Goal.Id)
                    return BuildPath();

                AddUpdate(Closed, node.Id, node);

                foreach (ProposedStep step in Grid.Neighbours(node))
                {
                    Node neighour = step.Node;

                    if (!Closed.ContainsKey(neighour.Id))
                    {
                        if (!Open.Contains(neighour))
                        {
                            AddUpdate(GHistory, neighour.Id, double.PositiveInfinity);
                            TryRemove(Parent, neighour);
                        }

                        double gOld = TryGetValue(GHistory, neighour.Id);
                        
                        if (TryGetValue(GHistory, node.Id) + step.Direction.Cost < TryGetValue(GHistory, neighour.Id))
                        {
                            AddUpdate(Parent, neighour, node);
                            AddUpdate(GHistory, neighour.Id, TryGetValue(GHistory, node.Id) + step.Direction.Cost);
                        }

                        if (TryGetValue(GHistory, neighour.Id) < gOld)
                        {
                            if (Open.Contains(neighour))
                                Open.Remove(neighour);

                            Open.Enqueue(neighour, TryGetValue(GHistory, neighour.Id) + Heuristic(neighour));
                        }
                    }
                }
            }

            return new List<Node>();
        }

        public void AddUpdate<TKey, TValue>(IDictionary<TKey, TValue> dictionary, TKey key, TValue value)
        {
            if (!dictionary.ContainsKey(key))
                dictionary.Add(key, value);
            else
                dictionary[key] = value;
        }

        public void TryRemove<TKey, TValue>(IDictionary<TKey, TValue> dictionary, TKey key)
        {
            if (dictionary.ContainsKey(key))
                dictionary.Remove(key);
        }

        public TValue TryGetValue<TKey, TValue>(IDictionary<TKey, TValue> dictionary, TKey key)
        {
            if (dictionary.ContainsKey(key))
                return dictionary[key];
            return default(TValue);
        }

        public IList<Node> BuildPath()
        {
            Node current = Goal;
            var path = new Stack<Node>();
            path.Push(current);
            do
            {
                current = Parent.First(n => n.Key.Id == current.Id).Value;
                path.Push(current);
            } while (current.Id != Start.Id);
            return path.ToList();
        }
    }

    public class Node : FastPriorityQueueNode
    {
        public readonly int X;
        public readonly int Y;

        public ulong Id;

        public Node(int x, int y)
        {
            X = x;
            Y = y;

            var sX = (ulong)x << 32;
            var sY = (ulong)y;

            Id = sX | sY;
        }

        public override string ToString()
        {
            return string.Format("[{0},{1}]", X, Y);
        }

        public class IdComparer : EqualityComparer<Node>
        {
            public override bool Equals(Node x, Node y)
            {
                return x.Id == y.Id;
            }

            public override int GetHashCode(Node obj)
            {
                return obj.Id.GetHashCode();
            }
        }
    }


}