using System;
using System.Collections.Generic;
using UnityEngine;

namespace WatStudios.DeepestDungeon.Utility.DataStructures
{
    public abstract class Heap<T> where T : IComparable<T>
    {
        #region Protected Fields
        protected List<T> _elements;
        protected int _size;
        #endregion

        #region Properties
        /// <summary>
        /// </summary>
        /// <param name="index">Source index</param>
        /// <returns>Index of the left child of <paramref name="index"/></returns>
        protected int LeftIndex(int index) => 2 * index + 1;
        /// <summary>
        /// </summary>
        /// <param name="index">Source index</param>
        /// <returns>Index of the right child of <paramref name="index"/></returns>
        protected int RightIndex(int index) => 2 * index + 2;
        /// <summary>
        /// </summary>
        /// <param name="index">Source index</param>
        /// <returns>Index of the parent of <paramref name="index"/></returns>
        protected int ParentIndex(int index) => (index - 1) / 2;
        protected bool HasLeftChild(int index) => LeftIndex(index) < _size;
        protected bool HasRightChild(int index) => RightIndex(index) < _size;
        protected bool IsRoot(int index) => index == 0;
        /// <summary>
        /// </summary>
        /// <param name="index">Source index</param>
        /// <returns>the left child as <typeparamref name="T"/> of <paramref name="index"/></returns>
        protected T LeftChild(int index) => _elements[LeftIndex(index)];
        /// <summary>
        /// </summary>
        /// <param name="index">Source index</param>
        /// <returns>the right child as <typeparamref name="T"/> of <paramref name="index"/></returns>
        protected T RightChild(int index) => _elements[RightIndex(index)];
        /// <summary>
        /// </summary>
        /// <param name="index">Source index</param>
        /// <returns>the parent as <typeparamref name="T"/> of <paramref name="index"/></returns>
        protected T Parent(int index) => _elements[ParentIndex(index)];
        public bool IsEmpty() => _size == 0;
        public int Count => _size;
        #endregion

        #region Protected Methods
        protected abstract void SiftDown();
        protected abstract void SiftUp();
        protected abstract bool IsHeap();

        /// <summary>
        /// Swaps the element at index <paramref name="firstIndex"/> with the element at index <paramref name="secondIndex"/>
        /// </summary>
        /// <param name="firstIndex"></param>
        /// <param name="secondIndex"></param>
        protected void Swap(int firstIndex, int secondIndex)
        {
            var temp = _elements[firstIndex];
            _elements[firstIndex] = _elements[secondIndex];
            _elements[secondIndex] = temp;
        }

        /// <summary>
        /// Turns the current _elements List into a heap
        /// </summary>
        protected void Heapify()
        {
            T[] temp = new T[_size];
            _elements.CopyTo(temp);
            _elements.Clear();
            _size = 0;
            foreach (var elem in temp)
            {
                Add(elem);
            }
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// </summary>
        /// <returns>The first element in the heap as Type <typeparamref name="T"/></returns>
        public T Peek()
        {
            if (_size == 0)
                throw new InvalidOperationException("Heap is empty");

            return _elements[0];
        }

        /// <summary>
        /// Removes the first element in the heap as Type <typeparamref name="T"/>
        /// </summary>
        /// <returns></returns>
        public T Pop()
        {
            if (_size == 0)
                throw new InvalidOperationException("Heap is empty");

            if (!IsHeap())
            {
                Heapify();
            }

            T result;
            if (_size > 2)
            {
                result = _elements[0];

                _elements[0] = _elements[_size - 1];
                _elements.RemoveAt(_size - 1);
                _size--;
                SiftDown();
            }
            else
            {
                result = _elements[0];
                _elements.RemoveAt(0);
                _size--;
            }

            return result;
        }

        /// <summary>
        /// Adds an element of Type <typeparamref name="T"/> to the Heap
        /// </summary>
        /// <param name="item">Element to add</param>
        public void Add(T item)
        {
            _elements.Add(item);
            _size++;

            SiftUp();
        }

        public override string ToString()
        {
            string result = "Heap: ";
            for (int i = 0; i < _size; i++)
            {
                result += _elements[i];
                if (i != _size - 1)
                    result += ", ";
            }
            return result;
        }
        #endregion
    }
}
