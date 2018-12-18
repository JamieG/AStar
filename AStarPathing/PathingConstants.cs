using System;

namespace AStarPathing
{
    public static class PathingConstants
    {
        // Octal distance costs
        public static double CardinalCost = 1f;
        public static double DiagonalCost = Math.Sqrt(2);

        // Bias > 1 will vastly increase performance at the cost of accuracy.
        public static double HeuristicBias = 1f;

        public static readonly StepDirection[] Directions =
        {
            // Cardinal
            new StepDirection(-1, +0, CardinalCost), // W
            new StepDirection(+1, +0, CardinalCost), // E
            new StepDirection(+0, +1, CardinalCost), // N 
            new StepDirection(+0, -1, CardinalCost), // S
            // Diagonal
            new StepDirection(-1, -1, DiagonalCost), // NW
            new StepDirection(-1, +1, DiagonalCost), // SW
            new StepDirection(+1, -1, DiagonalCost), // NE
            new StepDirection(+1, +1, DiagonalCost) // SE
        };
    }
}