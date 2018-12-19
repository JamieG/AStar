namespace AStar
{
    public interface IGrid
    {
        int Width { get; set; }
        int Height { get; set; }
        Cell this[int x, int y] { get; }
    }
}