namespace CapitalStaging
{
    public struct ProposedStep
    {
        public Node Node;
        public StepModifier Modifiers;

        public ProposedStep(Node node, StepModifier modifiers)
        {
            Node = node;
            Modifiers = modifiers;
        }
    }
}