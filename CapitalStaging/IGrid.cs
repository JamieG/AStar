namespace CapitalStaging
{
    using System.Collections.Generic;

    public interface IGrid
    {
        IEnumerable<ProposedStep> Neighbours(Node node);
        bool InBounds(Node proposedNode);
        int Width { get; set; }
        int Height { get; set; }
    }
}