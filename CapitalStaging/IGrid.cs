namespace CapitalStaging
{
    using System.Collections.Generic;

    public interface IGrid
    {
        IList<ProposedStep> Neighbours(Node node);
        int Cost(ProposedStep node);
    }
}