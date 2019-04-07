using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace JsonSchemaRoslyn.Core.Arrays
{
    /// <summary>
    /// Implements a circular buffer in an array by redefining array's accessors
    /// </summary>
    public abstract class CircularArray<T> : IEnumerable<T>, ICircularArray<T>
    {
        /// <summary>
        /// Stores the underlying items
        /// </summary>
        private T[] _mItems;

        /// <summary>
        /// Initializes a new instance of the CircularArray class
        /// </summary>
        /// <param name="size">The size of the array</param>
        protected CircularArray(int size)
        {
            this.SetSize(size);
        }

        /// <inheritdoc />
        public int Capacity => this._mItems.Length;
        
        /// <inheritdoc />
        public T this[int index]
        {
            get { return this._mItems[((index % this.Capacity) + this.Capacity) % this.Capacity]; }
            set { this._mItems[((index % this.Capacity) + this.Capacity) % this.Capacity] = value; }
        }
        
        /// <inheritdoc />
        public void Clear()
        {
            this.SetSize(this.Capacity);
        }

        /// <summary>
        /// Set the size of the internal buffer
        /// </summary>
        /// <param name="size">The size, in elements count, of the array</param>
        private void SetSize(int size)
        {
            if (size <= 0)
            {
                throw new ArgumentOutOfRangeException($"The size (current size: {size})of the list cannot be negative or equals to zero");
            }

            this._mItems = new T[size];
        }

        /// <summary>
        /// Get the buffered enumerator
        /// </summary>
        /// <returns></returns>
        public IEnumerator<T> GetEnumerator()
        {
            return this._mItems.AsEnumerable().GetEnumerator();
        }

        /// <summary>
        /// Get the buffered enumerator
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}