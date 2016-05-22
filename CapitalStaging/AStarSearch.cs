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

        public bool UseLineOfSight;

        public AStarSearch(IGrid grid, bool lineOfSight = false)
        {
            Grid = grid;
            UseLineOfSight = lineOfSight;

            Open = new NodePriorityQueue(Grid.Width * Grid.Height);
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

                        // Compute Cost

                        if (UseLineOfSight && LineOfSight(TryGetValue(Parent, node), neighour))
                        {
                            var sParent = TryGetValue(Parent, node);
                            var sParentCost = Cost(sParent, neighour);
                            if (TryGetValue(GHistory, sParent.Id) + sParentCost < TryGetValue(GHistory, neighour.Id))
                            {
                                AddUpdate(Parent, neighour, sParent);
                                AddUpdate(GHistory, neighour.Id, TryGetValue(GHistory, node.Id) + sParentCost);
                            }
                        }
                        else
                        {
                            if (TryGetValue(GHistory, node.Id) + step.Direction.Cost < TryGetValue(GHistory, neighour.Id))
                            {
                                AddUpdate(Parent, neighour, node);
                                AddUpdate(GHistory, neighour.Id, TryGetValue(GHistory, node.Id) + step.Direction.Cost);
                            }
                        }

                        // Compute Cost End

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

        private double Cost(Node a, Node b)
        {
            if (a.X == b.X || a.Y == b.Y)
                return 1d;
            if (a.X != b.X && a.Y != b.Y)
                return 1.4142135623730950488016887242097d;
            return 0d;
        }

        private bool LineOfSight(Node node, Node neighour)
        {
            int x0 = node.X;
            int y0 = node.Y;
            int x1 = neighour.X;
            int y1 = neighour.Y;
            int dy = y1 - y0;
            int dx = x1 - x0;

            int sy;
            int sx;
            int f = 0;

            if (dy < 0)
            {
                dy = -dy;
                sy = -1;
            }
            else
            {
                sy = 1;
            }

            if (dx < 0)
            {
                dx = -dx;
                sx = -1;
            }
            else
            {
                sx = 1;
            }

            if (dx >= dy)
            {
                while (x0 != x1)
                {
                    f += dy;
                    if (f >= dx)
                    {
                        if (Grid.Collided(x0 + (sx - 1) / 2, y0 + (sy - 1) / 2))
                            return false;
                        y0 += sy;
                        f -= dx;
                    }
                    if (f != 0 && Grid.Collided(x0 + (sx - 1) / 2, y0 + (sy - 1) / 2))
                        return false;
                    if (dy == 0 && Grid.Collided(x0 + (sx - 1) / 2, y0) && Grid.Collided(x0 + (sx - 1) / 2, y0 - 1))
                        return false;
                    x0 += sx;
                }
            }
            else
            {
                while (y0 != y1)
                {
                    f += dx;
                    if (f >= dy)
                    {
                        if (Grid.Collided(x0 + (sx - 1) / 2, y0 + (sy - 1) / 2))
                            return false;
                        x0 += sx;
                        f -= dy;
                    }
                    if (f != 0 && Grid.Collided(x0 + (sx - 1) / 2, y0 + (sy - 1) / 2))
                        return false;
                    if (dx == 0 && Grid.Collided(x0, y0 + (sy - 1) / 2) && Grid.Collided(x0 - 1, y0 + (sy - 1) / 2))
                        return false;
                    y0 += sy;
                }
            }
            return true;
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