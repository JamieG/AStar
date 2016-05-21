namespace CapitalStaging
{
    using System;
    using System.Collections.Generic;

    public class Grid : IGrid
    {
        private readonly short _boundsMinX;
        private readonly short _boundsMaxX;
        private readonly short _boundsMinY;
        private readonly short _boundsMaxY;

        public int Width { get; set; }
        public int Height { get; set; }

        public Grid(short width, short height)
        {
            Width = width;
            Height = height;

            _boundsMinX = 1;
            _boundsMaxX = (short) width;
            _boundsMinY = 1;
            _boundsMaxY = (short)height;
        }

        private static readonly StepDirection[] Directions = new[]
        {
            // Cardinal
            new StepDirection(-1, 0, 1), // W
            new StepDirection(1, 0, 1), // E
            new StepDirection(0, 1, 1), // N 
            new StepDirection(0, -1, 1), // S
            // Diagonal
            new StepDirection(-1, -1, 1.4142135623730950488016887242097), // NW
            new StepDirection(-1, 1, 1.4142135623730950488016887242097), // SW
            new StepDirection(1, -1, 1.4142135623730950488016887242097), // NE
            new StepDirection(1, 1, 1.4142135623730950488016887242097), // SE
        };

        private readonly Dictionary<ulong, Node> _knownNodes = new Dictionary<ulong, Node>(); 

        public IEnumerable<ProposedStep> Neighbours(Node node)
        {
            for (int i = 0; i < Directions.Length; i++)
            {
                var proposedNode = new Node(node.X + Directions[i].X, node.Y + Directions[i].Y);

                if (InBounds(proposedNode))
                {
                    Node neighour = proposedNode;

                    if (_knownNodes.ContainsKey(neighour.Id))
                        neighour = _knownNodes[neighour.Id];
                    else
                        _knownNodes.Add(neighour.Id, neighour);

                    yield return new ProposedStep(neighour, Directions[i]);
                }
            }
        }

        public bool InBounds(Node proposed)
        {
            return proposed.X >= _boundsMinX && proposed.X <= _boundsMaxX &&
                   proposed.Y >= _boundsMinY && proposed.Y <= _boundsMaxY;
        }

       
    }
}