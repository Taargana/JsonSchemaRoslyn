using System;

namespace JsonSchemaRoslyn.Core.Arrays
{
    /// <summary>
    /// Implements a generic data history
    /// </summary>
    public class History<T> : CircularArray<T>, IHistory<T>
    {
        /// <summary>
        /// Stores the count of the elements that already have been stored
        /// in the array
        /// </summary>
        private int _mCount;

        /// <summary>
        /// Stores the internal pointer, to know where to insert the next
        /// value in the array
        /// </summary>
        int _mPointer;

        /// <summary>
        /// Initializes a new instance of the History class
        /// </summary>
        /// <param name="size">The size of the history</param>
        public History(int size)
            : base(size)
        {
            this._mPointer = 0;
        }

        /// <inheritdoc />
        public int Count => this._mCount;

        /// <inheritdoc />
        public T Current => this[this._mPointer - 1];
        
        /// <inheritdoc />
        public virtual void Add(T item)
        {
            // We add the item to the current index pointer
            this[this._mPointer] = item;

            // We increment the pointer by one to prepair the next entry
            this._mPointer++;

            // If the pointer is greater than the capacity, decrement it by Capacity.
            // This operation is not really necessary as this[int] is taking care
            // of modulo operations but not doing this could lead to overflow exceptions
            // when running history for a long time.
            if (this._mPointer >= this.Capacity)
            {
                this._mPointer -= this.Capacity;
            }

            // Update the value of count property, if it is under the Capacity.
            // We do not need to increment it if the full capacity has been reached
            // because we are also removing the last element in this case.
            if (this._mCount < this.Capacity)
            {
                this._mCount++;
            }
        }

        /// <inheritdoc />
        public T Preceding(int value)
        {
            if (value <= 0)
            {
                throw new ArgumentOutOfRangeException($"value ({value}) can't be under or equals to 0.");
            }

            // The history doesn't store enough items, so return the default value for T
            if (value >= this.Count)
            {
                return default(T);
            }

            return (this[this._mPointer - 1 - value]);
        }
    }
}