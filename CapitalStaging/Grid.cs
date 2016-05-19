namespace CapitalStaging
{
    using System.Collections.Generic;

    public class Grid : IGrid
    {
        private readonly short _boundsMinX;
        private readonly short _boundsMaxX;
        private readonly short _boundsMinY;
        private readonly short _boundsMaxY;

        public Grid(short width, short height)
        {
            _boundsMinX = (short)-(width / 2);
            _boundsMaxX = (short)(width / 2);
            _boundsMinY = (short)-(height / 2);
            _boundsMaxY = (short)(height / 2);
        }

        private static readonly Node[] Directions = new[]
        {
            // Cardinal
            new Node(-1, 0), // W
            new Node(1, 0), // E
            new Node(0, 1), // N 
            new Node(0, -1), // S
            // Diagonal
            new Node(-1, -1), // NW
            new Node(-1, 1), // SW
            new Node(1, -1), // NE
            new Node(1, 1), // SE
        };

        public IList<ProposedStep> Neighbours(Node node)
        {

            var steps = new List<ProposedStep>();
            for (int i = 0; i < Directions.Length; i++)
            {
                var proposedNode = new Node((short)(node.X + Directions[i].X), (short)(node.Y + Directions[i].Y));
                if (InBounds(proposedNode))
                    steps.Add(new ProposedStep(proposedNode, i < 4 ? StepModifier.Cardinal : StepModifier.Diagonal));
            }
            return steps;
        }

        public int Cost(ProposedStep node)
        {
            int cost = 0;

            if (node.Modifiers.HasFlag(StepModifier.Cardinal))
                cost += 4;
            else if (node.Modifiers.HasFlag(StepModifier.Diagonal))
                cost += 6;

            return cost;
        }

        private bool InBounds(Node proposed)
        {
            return proposed.X >= _boundsMinX && proposed.X <= _boundsMaxX &&
                   proposed.Y >= _boundsMinY && proposed.Y <= _boundsMaxY;
        }
    }
}