using System.Collections.Generic;

namespace JsonSchemaRoslyn.Core.Arrays
{
    public interface IHistory<T> : IEnumerable<T>
    {
        /// <summary>
        /// Return the number of element in the history
        /// </summary>
        int Count { get; }
        /// <summary>
        /// Return the current element of the history
        /// </summary>
        T Current { get; }

        /// <summary>
        /// Add an item to the history
        /// </summary>
        /// <param name="item">The item to add to the history</param>
        void Add(T item);
        /// <summary>
        /// Get the value that was stored some frames ago
        /// </summary>
        ///<param name="value">The number of frame to look back</param>
        T Preceding(int value);
    }
}