using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private double Heuristic(Node node, Node goal)
        {
            var dX = Math.Abs(node.Location.X - goal.Location.X) * PathingConstants.HeuristicBias;
            var dY = Math.Abs(node.Location.Y - goal.Location.Y) * PathingConstants.HeuristicBias;

            // Octile distance
            return PathingConstants.CardinalCost * (dX + dY)
                   + (PathingConstants.DiagonalCost - 2 * PathingConstants.CardinalCost)
                   * Math.Min(dX, dY);
        }

        public Node[] Find(Node start, Node goal)
        {
            Parent.Add(start.Location.Id, start);
            Open.Enqueue(start, Heuristic(start, goal));

            while (Open.Count > 0)
            {
                var node = Open.Dequeue();

                if (node.Location.Id == goal.Location.Id)
                    return BuildPath(start, goal);

                Closed.AddUpdate(node.Location.Id, node);

                bool cBlock = false;

                for (var i = 0; i < PathingConstants.Directions.Length; i++)
                {
                    var direction = PathingConstants.Directions[i];

                    var proposed = new Vector2Int(
                        node.Location.X + direction.X,
                        node.Location.Y + direction.Y
                    );

                    if (!Grid.InBounds(proposed))
                        continue;

                    Node neighbour = Grid[proposed.X, proposed.Y];

                    if (neighbour.Blocked)
                    {
                        if (i < 4)
                            cBlock = true;
                        continue;
                    }

                    // Prevent slipping between blocked cardinals by an open diagonal
                    if (i >= 4 && cBlock)
                        continue;
                    
                    if (!Closed.ContainsKey(neighbour.Location.Id))
                    {
                        if (!Open.Contains(neighbour))
                        {
                            GHistory.AddUpdate(neighbour.Location.Id, double.PositiveInfinity);
                            Parent.TryRemove(neighbour.Location.Id);
                        }

                        var gOld = GHistory.TryGetValue(neighbour.Location.Id);

                        // Compute Cost
                        var g = GHistory.TryGetValue(node.Location.Id);
                        if (g + direction.Cost < gOld)
                        {
                            Parent.AddUpdate(neighbour.Location.Id, node);
                            GHistory.AddUpdate(neighbour.Location.Id, g + direction.Cost);
                        }

                        // Compute Cost End
                        var gNew = GHistory.TryGetValue(neighbour.Location.Id);
                        if (gNew < gOld)
                        {
                            var priority = gNew + Heuristic(neighbour, goal);
                            if (Open.Contains(neighbour))
                                Open.UpdatePriority(neighbour, priority);
                            else
                                Open.Enqueue(neighbour, priority);
                        }
                    }
                }
            }

            return new Node[0];
        }

        public Node[] BuildPath(Node start, Node goal)
        {
            var current = goal;
            var path = new Stack<Node>(new[] {current});
            while (current.Location.Id != start.Location.Id)
                path.Push(current = Parent[current.Location.Id]);
            return path.ToArray();
        }
    }
}