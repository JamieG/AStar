using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace AStar
{
    public struct CellData
    {
        public float G;
        public bool Closed;
        public Cell Parent;
    }

    public class AStarSearch
    {
        private readonly IGrid _grid;
        private readonly FastPriorityQueue<Cell> _open;

        public CellData[] Data { get; }

        public AStarSearch(IGrid grid)
        {
            _grid = grid;
            _open = new FastPriorityQueue<Cell>(_grid.Width * _grid.Height);
            Data = new CellData[_grid.Width * _grid.Height];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private float Heuristic(Cell cell, Cell goal)
        {
            var dX = Math.Abs(cell.Location.X - goal.Location.X) * PathingConstants.HeuristicBias;
            var dY = Math.Abs(cell.Location.Y - goal.Location.Y) * PathingConstants.HeuristicBias;

            // Octile distance
            return PathingConstants.CardinalCost * (dX + dY)
                   + (PathingConstants.DiagonalCost - 2 * PathingConstants.CardinalCost)
                   * Math.Min(dX, dY);
        }

        public Cell[] Find(Cell start, Cell goal)
        {
            Data[start.Id].Parent = start;
            _open.Enqueue(start, Heuristic(start, goal));

            while (_open.Count > 0)
            {
                var node = _open.Dequeue();

                if (node.Id == goal.Id)
                    return BuildPath(start, goal);

                Data[node.Id].Closed = true;

                var cBlock = false;

                for (var i = 0; i < PathingConstants.Directions.Length; i++)
                {
                    var direction = PathingConstants.Directions[i];

                    var proposed = new Vector2Int(
                        node.Location.X + direction.X,
                        node.Location.Y + direction.Y
                    );

                    // Bounds checking
                    if (proposed.X < 0 || proposed.X >= _grid.Width ||
                        proposed.Y < 0 || proposed.Y >= _grid.Height)
                        continue;

                    var neighbour = _grid[proposed.X, proposed.Y];

                    if (neighbour.Blocked)
                    {
                        if (i < 4)
                            cBlock = true;
                        continue;
                    }

                    // Prevent slipping between blocked cardinals by an open diagonal
                    if (i >= 4 && cBlock)
                        continue;

                    if (!Data[neighbour.Id].Closed)
                    {
                        if (!_open.Contains(neighbour))
                        {
                            Data[neighbour.Id].G = float.PositiveInfinity;
                            Data[neighbour.Id].Parent = null;
                        }

                        var gOld = Data[neighbour.Id].G;

                        // Compute Cost
                        var g = Data[node.Id].G;
                        if (g + direction.Cost < gOld)
                        {
                            Data[neighbour.Id].Parent = node;
                            Data[neighbour.Id].G = g + direction.Cost;
                        }

                        // Compute Cost End
                        var gNew = Data[neighbour.Id].G;
                        if (gNew < gOld)
                        {
                            var priority = gNew + Heuristic(neighbour, goal);
                            if (_open.Contains(neighbour))
                                _open.UpdatePriority(neighbour, priority);
                            else
                                _open.Enqueue(neighbour, priority);
                        }
                    }
                }
            }

            return new Cell[0];
        }

        private Cell[] BuildPath(Cell start, Cell goal)
        {
            var current = goal;
            var path = new Stack<Cell>(new[] {current});
            while (current.Id != start.Id)
                path.Push(current = Data[current.Id].Parent);
            return path.ToArray();
        }
    }
}