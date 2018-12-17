using System;

namespace AStarPathing
{
    [Flags]
    public enum StepModifier
    {
        None = 0,
        Cardinal = 1,
        Diagonal = 2
    }
}