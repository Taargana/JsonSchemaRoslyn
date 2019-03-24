using System;
using System.Text;
using JetBrains.Annotations;

namespace JsonSchemaRoslyn.Core.Extensions
{
    public class EncodingInfos
    {
        [NotNull] public Encoding Encoding { get; }
        public bool WithBom { get; }

        public static EncodingInfos CreateNew([NotNull]Encoding encoding, bool withBom)
        {
            if (encoding == null) throw new ArgumentNullException(nameof(encoding));
            return new EncodingInfos(encoding, withBom);
        }

        public EncodingInfos([NotNull]Encoding encoding, bool withBom)
        {
            Encoding = encoding ?? throw new ArgumentNullException(nameof(encoding));
            WithBom = withBom;
        }
    }
}