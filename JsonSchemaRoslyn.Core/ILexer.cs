using System;

namespace JsonSchemaRoslyn.Core
{
    public interface ILexer : IDisposable
    {
        SyntaxToken Lex();
    }
}