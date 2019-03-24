using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace JsonSchemaRoslyn.Core.Exceptions
{
    public class SyntaxKindException : Exception
    {
        public SyntaxKind Kind { get; }

        public SyntaxKindException(SyntaxKind kind) : this($"The provided {nameof(SyntaxKind)}.{kind} might not be allowed", kind)
        {
        }

        public SyntaxKindException(string message, SyntaxKind kind) : base(message)
        {
            Kind = kind;
        }

        public SyntaxKindException(string message, Exception innerException, SyntaxKind kind) : base(message, innerException)
        {
            Kind = kind;
        }

        protected SyntaxKindException(SerializationInfo info, StreamingContext context, SyntaxKind kind) : base(info, context)
        {
            Kind = kind;
        }
    }
}
