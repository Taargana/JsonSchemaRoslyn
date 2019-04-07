namespace JsonSchemaRoslyn.Core.Arrays
{
    /// <summary>
    /// Implements a double state History
    /// </summary>
    public class BufferedState<T> : History<T>, IBufferedState<T>
    {
        /// <summary>
        /// Initializes a new instance of the BufferedState class
        /// </summary>
        public BufferedState()
            : base(2)
        {
        }

        /// <inheritdoc />
        public T Previous => this.Preceding(1);
    }
}
