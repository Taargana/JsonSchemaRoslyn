using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace JsonSchemaRoslyn.Core
{
    public class DiagnosticBag : IEnumerable<Diagnostic>
    {
        [NotNull] private readonly ConcurrentBag<Diagnostic> _enumerableImplementation;

        public DiagnosticBag()
        {
            _enumerableImplementation = new ConcurrentBag<Diagnostic>();
        }

        public void AddDiagnostic([NotNull] Diagnostic diagnostic)
        {
            if (diagnostic == null) throw new ArgumentNullException(nameof(diagnostic));
            _enumerableImplementation.Add(diagnostic);
        }

        /// <inheritdoc />
        public IEnumerator<Diagnostic> GetEnumerator()
        {
            //mouai
            return _enumerableImplementation.OrderBy(d => d.Span.Start).GetEnumerator();
        }

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}