using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Xml;

namespace AStar {

    public enum SearchStatus {
        Idle,
        Searching,
        PathFound,
        NoPathFound
    }

    public static class Heuristics {

        // implementation for floating-point  Manhattan Distanceastrse
        public static double ManhattanDistance(double x1, double x2, double y1, double y2) {
            return Math.Abs(x1 - x2) + Math.Abs(y1 - y2);
        }

        // implementation for floating-point EuclideanDistance
        public static double EuclideanDistance(Vector2Int a, Vector2Int b) {
            var dx = Math.Abs(a.X - b.X);
            var dy = Math.Abs(a.Y - b.Y);

            return 1 * (dx + dy) + (2 - 2 * 1) * Math.Min(dx, dy);
        }

        // implementation for integer based Chebyshev Distance
        public static double ChebyshevDistance(double dx, double dy) {
            // not quite sure if the math is correct here
            return 1 * (dx + dy) + (1 - 2 * 1) * (dx - dy);
        }
    }

    public class AStarSearch {
        private IGridProvider _grid;
        private FastPriorityQueue _open;

        public SearchStatus Status { get; private set; }

        public void Initialise(IGridProvider grid) {
            _grid = grid;
            _open = new FastPriorityQueue(_grid.Size.X * _grid.Size.Y);

            Status = SearchStatus.Idle;
        }

        private double Heuristic(Cell cell, Cell goal) {
            var dX = Math.Abs(cell.Location.X - goal.Location.X);
            var dY = Math.Abs(cell.Location.Y - goal.Location.Y);

            // Octile distance
            return 1 * (dX + dY) 
                   + (Math.Sqrt(2) - 2 * 1) 
                   * Math.Min(dX, dY);
        }

        public void Reset() {

            _open.Clear();
        }

        public Cell[] Find(Cell start, Cell goal) {

            _open.Enqueue(start, 0);

            var bounds = _grid.Size;

            Cell node = null;

            while (_open.Count > 0) {

                node = _open.Dequeue();

                node.Closed = true;
                //_grid[node.Location].Closed = true;

                var cBlock = false;

                var g = node.G + 1;

                if (goal.Location == node.Location) {
                    break;
                }

                Vector2Int proposed = new Vector2Int(0, 0);

                for (var i = 0; i < PathingConstants.Directions.Length; i++) {

                    var direction = PathingConstants.Directions[i];

                    proposed.X = node.Location.X + direction.X;
                    proposed.Y = node.Location.Y + direction.Y;
                    
                    // Bounds checking
                    if (proposed.X < 0 || proposed.X >= bounds.X ||
                        proposed.Y < 0 || proposed.Y >= bounds.Y)
                        continue;

                    Cell neighbour = _grid[proposed];

                    if (neighbour.Blocked) {

                        if (i < 4)
                        {
                            cBlock = true;
                        }

                        continue;
                    }

                    // Prevent slipping between blocked cardinals by an open diagonal
                    if (i >= 4 && cBlock)
                    {
                        continue;
                    }

                    if (_grid[neighbour.Location].Closed) {

                        continue;
                    }

                    if (!_open.Contains(neighbour)) {

                        neighbour.G = g;
                        neighbour.H = Heuristic(neighbour, node);
                        _open.Enqueue(neighbour, neighbour.G + neighbour.H);
                        neighbour.Parent = node;

                        
                    } else {
                        if (g + neighbour.H < neighbour.F) {

                            neighbour.G = g;
                            neighbour.F = neighbour.G + neighbour.H;
                            neighbour.Parent = node;
                        }
                    }
                }
            }

            var path = new Stack<Cell>();

            while (node != null) {
                path.Push(node);
                node = node.Parent;
            }

            return path.ToArray();
        }
    }

}