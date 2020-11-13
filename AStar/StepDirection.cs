namespace AStar
{
    public readonly struct StepDirection
    {
        public readonly int X;
        public readonly int Y;
        public readonly float Cost;

        public StepDirection(int x, int y, float cost)
        {
            X = x;
            Y = y;
            Cost = cost;
        }
    }
}