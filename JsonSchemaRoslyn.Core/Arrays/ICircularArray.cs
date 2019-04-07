namespace JsonSchemaRoslyn.Core.Arrays
{
    public interface ICircularArray<T>
    {
        /// <summary>
        /// Gets the capacity of the underlying array
        /// </summary>
        int Capacity { get; }

        /// <summary>
        /// Gets or sets the array at an index
        /// </summary>
        /// <param name="index"></param>
        /// <returns>The object of type T at the modulated index</returns>
        T this[int index] { get; set; }

        /// <summary>
        /// Clear the array
        /// </summary>
        void Clear();
    }
}