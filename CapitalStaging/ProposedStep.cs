namespace CapitalStaging
{
    public struct ProposedStep
    {
        public Node Node;
        public StepDirection Direction;

        public ProposedStep(Node node, StepDirection direction)
        {
            Node = node;
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