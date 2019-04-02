using System;
using System.Runtime.Serialization;

namespace JsonSchemaRoslyn.Core.Exceptions
{
    public class EndOfFileExtractLiteralException : ExtractLiteralException
    {
        public EndOfFileExtractLiteralException()
        {
        }

        public EndOfFileExtractLiteralException(string message) : base(message)
        {
        }

        public EndOfFileExtractLiteralException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public EndOfFileExtractLiteralException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}