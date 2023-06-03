using System;

namespace WatStudios.DeepestDungeon.Utility.DataStructures
{
    /// <summary>
    /// This custom List implements a PriorityQueue in which items are sorted according to their key.
    /// The List will be sorted in an ascending or a descending order.
    /// </summary>
    /// <typeparam name="T">Type for the generic PriorityQueue</typeparam>
    public class PriorityQueue<T> where T : IComparable<T>
    {
        #region Private Fields
        private Heap<T> _heap;
        #endregion

        #region Properties
        public int Count => _heap.Count;
        #endregion

        public PriorityQueue(bool sortAscending)
        {
            if (sortAscending)
                _heap = new MinHeap<T>();
            else
                _heap = new MaxHeap<T>();
        }

        #region Public Methods
        /// <summary>
        /// Adds the item at the correct Position according to the <paramref name="item"/> and according to the value in SortAscending.
        /// </summary>
        /// <param name="item"></param>
        public void Enqueue(T item)
        {
            _heap.Add(item);
        }

        /// <summary>
        /// Removes the first element in the queue and returns it
        /// </summary>
        /// <returns>Element of type <typeparamref name="T"/></returns>
        public T Dequeue()
        {
            return _heap.Pop();
        }

        /// <summary>
        /// Returns the first element in the queue
        /// </summary>
        /// <returns>Element of type <typeparamref name="T"/></returns>
        public T Peek()
        {
            return _heap.Peek();
        }

        public override string ToString()
        {
            return _heap.ToString();
        }
        #endregion
    }
}
