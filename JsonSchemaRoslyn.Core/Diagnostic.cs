using System;

namespace JsonSchemaRoslyn.Core
{
    public sealed class Diagnostic
    {
        public Diagnostic(TextSpan span, string message)
        {
            Span = span;
            Message = message;
        }
        public Diagnostic(TextSpan span, string message, Exception exception = null)
        {
            Span = span;
            Message = message;
            Exception = exception;
        }

        public TextSpan Span { get; }
        public string Message { get; }
        public Exception Exception { get; }

        public override string ToString() => Message;
    }
}