namespace JsonSchemaRoslyn.Core
{
    public enum SyntaxKind
    {
        Unknown = -1,
        EndOfFile,
        OpenObjectCurlyBracket,
        Whitespace,
        DoubleQuote,
        CloseObjectCurlyBracket,
        Letter,
        Digit,
        Colon,
        Star,
        BackSlash,
        Slash,
        Equal,
        SemiColon,
        Coma,
        Minus,
        SimpleQuote,
        Dot,
        Plus,
        Literal,
        PropertyName,
        OpenArrayBracket,
        CloseArrayBracket,
        Boolean,
        Null,
        Count
    }
}