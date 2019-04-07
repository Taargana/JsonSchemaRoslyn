namespace JsonSchemaRoslyn.Core.Arrays
{
    public interface IBufferedState<T> : IHistory<T>
    {
        /// <summary>
        /// Gets the previous value
        /// </summary>
        T Previous { get; }
    }
}