namespace CapitalStaging
{
    using System.Collections.Generic;

    public interface IGrid
    {
        ProposedStep[] Neighbours(Node node);
        int Width { get; set; }
        int Height { get; set; }
        bool InBounds(Vector2Int proposed);

        Node this[int x, int y] { get; }
        Node this[Vector2Int location] { get; }
    }
}