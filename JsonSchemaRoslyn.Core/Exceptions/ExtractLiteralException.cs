using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace JsonSchemaRoslyn.Core.Exceptions
{
    public abstract class ExtractLiteralException : Exception
    {
        protected ExtractLiteralException()
        {
        }

        protected ExtractLiteralException(string message) : base(message)
        {
        }

        protected ExtractLiteralException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected ExtractLiteralException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
