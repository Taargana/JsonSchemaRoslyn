namespace JsonSchemaRoslyn.Core
{
    public class SyntaxToken
    {
        /// <summary>
        /// Syntax token kind
        /// </summary>
        public SyntaxKind Kind { get; }
        /// <summary>
        /// Raw content of the syntax token
        /// </summary>
        public string Text { get; }
        /// <summary>
        /// position of the token in the original text
        /// </summary>
        public long StartPosition { get; }
        /// <summary>
        /// Real value of the token; for instance if the token is of type integer then the value is an integer
        /// </summary>
        public object Value { get; }

        private SyntaxToken(){}

        /// <summary>
        /// Return a new SyntaxToken
        /// </summary>
        internal static SyntaxToken CreateNew(SyntaxKind kind, string text, int startPosition, object value = null)
        {
            return new SyntaxToken(kind,text,startPosition, value);
        }
        
        /// <summary>
        /// Return a new SyntaxToken
        /// </summary>
        internal SyntaxToken(SyntaxKind kind, string text, long startPosition, object value = null)
        {
            Kind = kind;
            Text = text;
            StartPosition = startPosition;
            Value = value;
        }
    }
}