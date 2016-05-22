namespace CapitalStaging
{
    public class CostCalculator
    {
        private readonly AStarSearch _aStarSearch;

        public CostCalculator(AStarSearch aStarSearch)
        {
            _aStarSearch = aStarSearch;
        }

        public void ComputeCost(Node neighour, Node node, ProposedStep step)
        {
            if (_aStarSearch.TryGetValue(_aStarSearch.GHistory, node.Id) + step.Direction.Cost < _aStarSearch.TryGetValue(_aStarSearch.GHistory, neighour.Id))
            {
                _aStarSearch.AddUpdate(_aStarSearch.Parent, neighour, node);
                _aStarSearch.AddUpdate(_aStarSearch.GHistory, neighour.Id, _aStarSearch.TryGetValue(_aStarSearch.GHistory, node.Id) + step.Direction.Cost);
            }
        }
    }
}