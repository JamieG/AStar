namespace CapitalStaging
{
    public class ProposedStep
    {
        public Node Node { get; set; }
        public StepDirection Direction { get; private set; }

        public ProposedStep(StepDirection direction)
        {
            Direction = direction;
        }
    }

    public struct StepDirection
    {
        public readonly int X;
        public readonly int Y;
        public readonly double Cost;
        

        public StepDirection(int x, int y, double cost)
        {
            X = x;
            Y = y;
            Cost = cost;
        }
    }
}