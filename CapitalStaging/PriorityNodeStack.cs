namespace CapitalStaging
{
    using System.Collections.Generic;
    using System.Linq;

    public class PriorityNodeStack
    {
        private readonly Dictionary<int, Node> _known = new Dictionary<int, Node>();
        private readonly Dictionary<int, Stack<Node>> _inner = new Dictionary<int, Stack<Node>>();

        public virtual void Push(int priority, Node item)
        {
            if (!_inner.ContainsKey(priority))
                _inner.Add(priority, new Stack<Node>());
            _known.Add(item.Key, item);
            _inner[priority].Push(item);
        }

        public virtual Node Pop()
        {
            var stackPair = _inner.First();
            var item = stackPair.Value.Pop();
            _known.Remove(item.Key);
            if (stackPair.Value.Count == 0)
                _inner.Remove(stackPair.Key);
            return item;
        }

        public bool Any()
        {
            return _inner.Any();
        }

        public bool Contains(int key)
        {
            return _known.ContainsKey(key);
        }
    }
}