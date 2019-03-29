using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace JsonSchemaRoslyn.Core
{
    public interface ILexer : IDisposable
    {
        [NotNull] IEnumerable<SyntaxToken> Lex();
    }
}