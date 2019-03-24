using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace JsonSchemaRoslyn.Core
{
    public class DiagnosticBag : IEnumerable<Diagnostic>
    {
        [NotNull] private readonly List<Diagnostic> _enumerableImplementation;

        public DiagnosticBag()
        {
            _enumerableImplementation = new List<Diagnostic>();
        }

        public void AddDiagnostic(Diagnostic diagnostic)
        {
            _enumerableImplementation.Add(diagnostic);
        }

        /// <inheritdoc />
        public IEnumerator<Diagnostic> GetEnumerator()
        {
            return _enumerableImplementation.GetEnumerator();
        }

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable) _enumerableImplementation).GetEnumerator();
        }
    }
}