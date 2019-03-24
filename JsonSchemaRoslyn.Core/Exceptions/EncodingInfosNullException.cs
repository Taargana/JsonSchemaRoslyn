using System;
using System.Runtime.Serialization;

namespace JsonSchemaRoslyn.Core.Exceptions
{
    public class EncodingInfosNullException : Exception
    {
        public EncodingInfosNullException()
        {
        }

        public EncodingInfosNullException(string message) : base(message)
        {
        }

        public EncodingInfosNullException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected EncodingInfosNullException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}