using System;
using System.Collections.Generic;
using System.Linq;

namespace WatStudios.DeepestDungeon.Utility.DataStructures
{
    public class MinHeap<T> : Heap<T> where T : IComparable<T>
    {
        public MinHeap()
        {
            _elements = new List<T>();
            _size = _elements.Count;
        }

        public MinHeap(T[] array)
        {
            _elements = array.ToList();
            _size = _elements.Count;
            Heapify();
        }

        public MinHeap(ICollection<T> collection)
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
                var smallerIndex = LeftIndex(index);
                if (HasRightChild(index) && RightChild(index).CompareTo(LeftChild(index)) < 0)
                {
                    smallerIndex = RightIndex(index);
                }

                if (_elements[smallerIndex].CompareTo(_elements[index]) >= 0)
                {
                    break;
                }

                Swap(smallerIndex, index);
                index = smallerIndex;
            }
        }

        protected override void SiftUp()
        {
            var index = _size - 1;
            while (!IsRoot(index) && _elements[index].CompareTo(Parent(index)) < 0)
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

                if (leftChildIndex <= lastIndex && _elements[currentIndex].CompareTo(_elements[leftChildIndex]) >= 0)
                    return false;
                if (rightChildIndex <= lastIndex && _elements[currentIndex].CompareTo(_elements[rightChildIndex]) >= 0)
                    return false;
            }
            return true;
        }
    }
}
