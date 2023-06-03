using System;
using System.Collections.Generic;
using System.Linq;

namespace WatStudios.DeepestDungeon.Utility.DataStructures
{
    public class MaxHeap<T> : Heap<T> where T : IComparable<T>
    {
        public MaxHeap()
        {
            _elements = new List<T>();
            _size = _elements.Count;
        }

        public MaxHeap(T[] array)
        {
            _elements = array.ToList();
            _size = _elements.Count;
            Heapify();
        }

        public MaxHeap(ICollection<T> collection)
        {
            _elements = collection.ToList();
            _size = _elements.Count;
            Heapify();
        }

        protected override void SiftDown()
        {
            int index = 0;
            while (HasLeftChild(index))
            {
                var biggerIndex = LeftIndex(index);
                if (HasRightChild(index) && RightChild(index).CompareTo(LeftChild(index)) > 0)
                {
                    biggerIndex = RightIndex(index);
                }

                if (_elements[biggerIndex].CompareTo(_elements[index]) <= 0)
                {
                    break;
                }

                Swap(biggerIndex, index);
                index = biggerIndex;
            }
        }

        protected override void SiftUp()
        {
            var index = _size - 1;
            while (!IsRoot(index) && _elements[index].CompareTo(Parent(index)) > 0)
            {
                var parentIndex = ParentIndex(index);
                Swap(parentIndex, index);
                index = parentIndex;
            }
        }

        protected override bool IsHeap()
        {
            if (_size == 0)
                return true;

            int lastIndex = _size - 1;
            for (int currentIndex = 0; currentIndex < _size; currentIndex++)
            {
                var leftChildIndex = LeftIndex(currentIndex);
                var rightChildIndex = RightIndex(currentIndex);

                if (leftChildIndex <= lastIndex && _elements[currentIndex].CompareTo(_elements[leftChildIndex]) <= 0)
                    return false;
                if (rightChildIndex <= lastIndex && _elements[currentIndex].CompareTo(_elements[rightChildIndex]) <= 0)
                    return false;
            }
            return true;
        }
    }
}
