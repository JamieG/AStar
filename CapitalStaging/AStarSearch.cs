namespace CapitalStaging
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class AStarSearch
    {
        private readonly PriorityNodeStack _open;
        private readonly Dictionary<int, Node> _closed;
        private readonly IGrid _grid;
        private readonly Node _start;
        private readonly Node _goal;
        private readonly Dictionary<int, int> _gScore;

        public AStarSearch(IGrid grid, Node start, Node goal)
        {
            _open = new PriorityNodeStack();
            _closed = new Dictionary<int, Node>();
            _gScore = new Dictionary<int, int>();

            _grid = grid;
            _start = start;
            _goal = goal;

            _gScore.Add(_start.Key, 0);
            _open.Push(Distance(_start, _goal), _start);
        }

        private int Distance(Node from, Node to)
        {
            return (int)Math.Sqrt(Math.Pow(from.X - to.X, 2) + Math.Pow(from.Y - to.Y, 2));
        }

        public IList<Node> Find()
        {
            var cameFrom = new Dictionary<Node, Node>();

            int maxSearch = 1000000;

            while (_open.Any() && maxSearch >= 0)
            {
                Node current = _open.Pop();

                if (current == Node.Empty)
                    throw new ApplicationException("Pathing error, lowest f score not found in open set!");

                // Path found!
                if (current.Key == _goal.Key)
                    return BuildPath(cameFrom, current);

                _closed.Add(current.Key, current);

                foreach (ProposedStep proposedStep in _grid.Neighbours(current))
                {
                    Node neighbor = proposedStep.Node;

                    if (_closed.ContainsKey(neighbor.Key))
                        continue;

                    int gScore = _gScore[current.Key] + Distance(current, neighbor);

                    if (!_open.Contains(neighbor.Key))
                    {
                        int fScore = gScore + _grid.Cost(proposedStep) + Distance(neighbor, _goal);
                        _open.Push(fScore, neighbor);
                    }
                    else if (gScore >= _gScore[neighbor.Key])
                    {
                        continue; 
                    }

                    cameFrom[neighbor] = current;
                    _gScore[neighbor.Key] = gScore;
                }

                maxSearch--;

            }

            return new List<Node>();
        }
        private IList<Node> BuildPath(Dictionary<Node, Node> cameFrom, Node current)
        {
            var path = new Stack<Node>();
            path.Push(current);
            while (cameFrom.ContainsKey(current))
            {
                current = cameFrom[current];
                path.Push(current);
            }
            return path.ToList();
        }
    }
}