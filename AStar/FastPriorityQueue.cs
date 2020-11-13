using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace AStar
{
    /// <summary>
    ///     An implementation of a min-Priority Queue using a heap.  Has O(1) .Contains()!
    ///     See https://github.com/BlueRaja/High-Speed-Priority-Queue-for-C-Sharp/wiki/Getting-Started for more information
    /// </summary>
    /// <typeparam name="T">The values in the queue.  Must extend the FastPriorityQueueNode class</typeparam>
    public sealed class FastPriorityQueue : IPriorityQueue<Cell, double>
    {
        private Cell[] _nodes;

        /// <summary>
        ///     Returns the maximum number of items that can be enqueued at once in this queue.  Once you hit this number (ie. once
        ///     Count == MaxSize),
        ///     attempting to enqueue another item will cause undefined behavior.  O(1)
        /// </summary>
        public int MaxSize => _nodes.Length - 1;

        /// <summary>
        ///     Returns the number of nodes in the queue.
        ///     O(1)
        /// </summary>
        public int Count { get; private set; }

        /// <summary>
        ///     Returns the head of the queue, without removing it (use Dequeue() for that).
        ///     If the queue is empty, behavior is undefined.
        ///     O(1)
        /// </summary>
        public Cell First => _nodes[1];

        /// <summary>
        ///     Instantiate a new Priority Queue
        /// </summary>
        /// <param name="maxNodes">The max nodes ever allowed to be enqueued (going over this will cause undefined behavior)</param>
        public FastPriorityQueue(int maxNodes)
        {
            Count = 0;
            _nodes = new Cell[maxNodes + 1];
        }

        /// <summary>
        ///     Removes every node from the queue.
        ///     O(n) (So, don't do this often!)
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Clear()
        {
            Array.Clear(_nodes, 1, Count);
            Count = 0;
        }

        /// <summary>
        ///     Returns (in O(1)!) whether the given node is in the queue.
        ///     If node is or has been previously added to another queue, the result is undefined unless oldQueue.ResetNode(node)
        ///     has been called
        ///     O(1)
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Contains(Cell node) => _nodes[node.QueueIndex] == node;

        /// <summary>
        ///     Enqueue a node to the priority queue.  Lower values are placed in front. Ties are broken arbitrarily.
        ///     If the queue is full, the result is undefined.
        ///     If the node is already enqueued, the result is undefined.
        ///     If node is or has been previously added to another queue, the result is undefined unless oldQueue.ResetNode(node)
        ///     has been called
        ///     O(log n)
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Enqueue(Cell node, double priority)
        {
            node.F = priority;
            Count++;
            _nodes[Count] = node;
            node.QueueIndex = Count;
            CascadeUp(node);
        }

        /// <summary>
        ///     Removes the head of the queue and returns it.
        ///     If queue is empty, result is undefined
        ///     O(log n)
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Cell Dequeue()
        {
            var returnMe = _nodes[1];
            //If the node is already the last node, we can remove it immediately
            if (Count == 1)
            {
                _nodes[1] = null;
                Count = 0;
                return returnMe;
            }

            //Swap the node with the last node
            var formerLastNode = _nodes[Count];
            _nodes[1] = formerLastNode;
            formerLastNode.QueueIndex = 1;
            _nodes[Count] = null;
            Count--;

            //Now bubble formerLastNode (which is no longer the last node) down
            CascadeDown(formerLastNode);
            return returnMe;
        }

        /// <summary>
        ///     This method must be called on a node every time its priority changes while it is in the queue.
        ///     <b>Forgetting to call this method will result in a corrupted queue!</b>
        ///     Calling this method on a node not in the queue results in undefined behavior
        ///     O(log n)
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void UpdatePriority(Cell node, double priority)
        {
            node.F = priority;
            OnNodeUpdated(node);
        }

        /// <summary>
        ///     Removes a node from the queue.  The node does not need to be the head of the queue.
        ///     If the node is not in the queue, the result is undefined.  If unsure, check Contains() first
        ///     O(log n)
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Remove(Cell node)
        {
            //If the node is already the last node, we can remove it immediately
            if (node.QueueIndex == Count)
            {
                _nodes[Count] = null;
                Count--;
                return;
            }

            //Swap the node with the last node
            var formerLastNode = _nodes[Count];
            _nodes[node.QueueIndex] = formerLastNode;
            formerLastNode.QueueIndex = node.QueueIndex;
            _nodes[Count] = null;
            Count--;

            //Now bubble formerLastNode (which is no longer the last node) up or down as appropriate
            OnNodeUpdated(formerLastNode);
        }

        public IEnumerator<Cell> GetEnumerator()
        {
            IEnumerable<Cell> e = new ArraySegment<Cell>(_nodes, 1, Count);
            return e.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void CascadeUp(Cell node)
        {
            //aka Heapify-up
            int parent;
            if (node.QueueIndex > 1)
            {
                parent = node.QueueIndex >> 1;
                var parentNode = _nodes[parent];
                if (HasHigherOrEqualPriority(parentNode, node))
                    return;

                //Node has lower priority value, so move parent down the heap to make room
                _nodes[node.QueueIndex] = parentNode;
                parentNode.QueueIndex = node.QueueIndex;

                node.QueueIndex = parent;
            }
            else
            {
                return;
            }

            while (parent > 1)
            {
                parent >>= 1;
                var parentNode = _nodes[parent];
                if (HasHigherOrEqualPriority(parentNode, node))
                    break;

                //Node has lower priority value, so move parent down the heap to make room
                _nodes[node.QueueIndex] = parentNode;
                parentNode.QueueIndex = node.QueueIndex;

                node.QueueIndex = parent;
            }

            _nodes[node.QueueIndex] = node;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void CascadeDown(Cell node)
        {
            //aka Heapify-down
            var finalQueueIndex = node.QueueIndex;
            var childLeftIndex = 2 * finalQueueIndex;

            // If leaf node, we're done
            if (childLeftIndex > Count) return;

            // Check if the left-child is higher-priority than the current node
            var childRightIndex = childLeftIndex + 1;
            var childLeft = _nodes[childLeftIndex];
            if (HasHigherPriority(childLeft, node))
            {
                // Check if there is a right child. If not, swap and finish.
                if (childRightIndex > Count)
                {
                    node.QueueIndex = childLeftIndex;
                    childLeft.QueueIndex = finalQueueIndex;
                    _nodes[finalQueueIndex] = childLeft;
                    _nodes[childLeftIndex] = node;
                    return;
                }

                // Check if the left-child is higher-priority than the right-child
                var childRight = _nodes[childRightIndex];
                if (HasHigherPriority(childLeft, childRight))
                {
                    // left is highest, move it up and continue
                    childLeft.QueueIndex = finalQueueIndex;
                    _nodes[finalQueueIndex] = childLeft;
                    finalQueueIndex = childLeftIndex;
                }
                else
                {
                    // right is even higher, move it up and continue
                    childRight.QueueIndex = finalQueueIndex;
                    _nodes[finalQueueIndex] = childRight;
                    finalQueueIndex = childRightIndex;
                }
            }
            // Not swapping with left-child, does right-child exist?
            else if (childRightIndex > Count)
            {
                return;
            }
            else
            {
                // Check if the right-child is higher-priority than the current node
                var childRight = _nodes[childRightIndex];
                if (HasHigherPriority(childRight, node))
                {
                    childRight.QueueIndex = finalQueueIndex;
                    _nodes[finalQueueIndex] = childRight;
                    finalQueueIndex = childRightIndex;
                }
                // Neither child is higher-priority than current, so finish and stop.
                else
                {
                    return;
                }
            }

            while (true)
            {
                childLeftIndex = 2 * finalQueueIndex;

                // If leaf node, we're done
                if (childLeftIndex > Count)
                {
                    node.QueueIndex = finalQueueIndex;
                    _nodes[finalQueueIndex] = node;
                    break;
                }

                // Check if the left-child is higher-priority than the current node
                childRightIndex = childLeftIndex + 1;
                childLeft = _nodes[childLeftIndex];
                if (HasHigherPriority(childLeft, node))
                {
                    // Check if there is a right child. If not, swap and finish.
                    if (childRightIndex > Count)
                    {
                        node.QueueIndex = childLeftIndex;
                        childLeft.QueueIndex = finalQueueIndex;
                        _nodes[finalQueueIndex] = childLeft;
                        _nodes[childLeftIndex] = node;
                        break;
                    }

                    // Check if the left-child is higher-priority than the right-child
                    var childRight = _nodes[childRightIndex];
                    if (HasHigherPriority(childLeft, childRight))
                    {
                        // left is highest, move it up and continue
                        childLeft.QueueIndex = finalQueueIndex;
                        _nodes[finalQueueIndex] = childLeft;
                        finalQueueIndex = childLeftIndex;
                    }
                    else
                    {
                        // right is even higher, move it up and continue
                        childRight.QueueIndex = finalQueueIndex;
                        _nodes[finalQueueIndex] = childRight;
                        finalQueueIndex = childRightIndex;
                    }
                }
                // Not swapping with left-child, does right-child exist?
                else if (childRightIndex > Count)
                {
                    node.QueueIndex = finalQueueIndex;
                    _nodes[finalQueueIndex] = node;
                    break;
                }
                else
                {
                    // Check if the right-child is higher-priority than the current node
                    var childRight = _nodes[childRightIndex];
                    if (HasHigherPriority(childRight, node))
                    {
                        childRight.QueueIndex = finalQueueIndex;
                        _nodes[finalQueueIndex] = childRight;
                        finalQueueIndex = childRightIndex;
                    }
                    // Neither child is higher-priority than current, so finish and stop.
                    else
                    {
                        node.QueueIndex = finalQueueIndex;
                        _nodes[finalQueueIndex] = node;
                        break;
                    }
                }
            }
        }

        /// <summary>
        ///     Returns true if 'higher' has higher priority than 'lower', false otherwise.
        ///     Note that calling HasHigherPriority(node, node) (ie. both arguments the same node) will return false
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool HasHigherPriority(Cell higher, Cell lower) => higher.F < lower.F;

        /// <summary>
        ///     Returns true if 'higher' has higher priority than 'lower', false otherwise.
        ///     Note that calling HasHigherOrEqualPriority(node, node) (ie. both arguments the same node) will return true
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool HasHigherOrEqualPriority(Cell higher, Cell lower) => higher.F <= lower.F;

        /// <summary>
        ///     Resize the queue so it can accept more nodes.  All currently enqueued nodes are remain.
        ///     Attempting to decrease the queue size to a size too small to hold the existing nodes results in undefined behavior
        ///     O(n)
        /// </summary>
        public void Resize(int maxNodes)
        {
            var newArray = new Cell[maxNodes + 1];
            var highestIndexToCopy = Math.Min(maxNodes, Count);
            Array.Copy(_nodes, newArray, highestIndexToCopy + 1);
            _nodes = newArray;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void OnNodeUpdated(Cell node)
        {
            //Bubble the updated node up or down as appropriate
            var parentIndex = node.QueueIndex >> 1;

            if (parentIndex > 0 && HasHigherPriority(node, _nodes[parentIndex]))
                CascadeUp(node);
            else
                CascadeDown(node);
        }

        /// <summary>
        ///     By default, nodes that have been previously added to one queue cannot be added to another queue.
        ///     If you need to do this, please call originalQueue.ResetNode(node) before attempting to add it in the new queue
        ///     If the node is currently in the queue or belongs to another queue, the result is undefined
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ResetNode(Cell node)
        {
            node.QueueIndex = 0;
        }

        /// <summary>
        ///     <b>Should not be called in production code.</b>
        ///     Checks to make sure the queue is still in a valid state.  Used for testing/debugging the queue.
        /// </summary>
        public bool IsValidQueue()
        {
            for (var i = 1; i < _nodes.Length; i++)
                if (_nodes[i] != null)
                {
                    var childLeftIndex = 2 * i;
                    if (childLeftIndex < _nodes.Length && _nodes[childLeftIndex] != null &&
                        HasHigherPriority(_nodes[childLeftIndex], _nodes[i]))
                        return false;

                    var childRightIndex = childLeftIndex + 1;
                    if (childRightIndex < _nodes.Length && _nodes[childRightIndex] != null &&
                        HasHigherPriority(_nodes[childRightIndex], _nodes[i]))
                        return false;
                }

            return true;
        }
    }
}