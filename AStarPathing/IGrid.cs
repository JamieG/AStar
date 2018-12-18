namespace AStarPathing
{
    public interface IGrid
    {
        int Width { get; set; }
        int Height { get; set; }
        Node this[int x, int y] { get; }
        Node this[Vector2Int location] { get; }
        bool InBounds(Vector2Int proposed);
    }
}