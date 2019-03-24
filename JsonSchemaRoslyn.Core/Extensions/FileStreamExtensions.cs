using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using JetBrains.Annotations;

namespace JsonSchemaRoslyn.Core.Extensions
{
    public static class FileStreamExtensions
    {
        /// <summary>
        /// Determines a text file's encoding by analyzing its byte order mark (BOM).
        /// Defaults to ASCII when detection of the text file's endianness fails.
        /// </summary>
        /// <returns>The detected encoding.</returns>
        public static EncodingInfos GetEncoding([NotNull] this Stream sourceFile)
        {
            if (sourceFile == null) throw new ArgumentNullException(nameof(sourceFile));
            // Read the BOM
            var bom = new byte[3];
            sourceFile.Read(bom, 0, 3);
            
            // Analyze the BOM
            switch (bom[0])
            {
                case 0x2b when bom[1] == 0x2f && bom[2] == 0x76:
                    return EncodingInfos.CreateNew(Encoding.UTF7, true);
                case 0xef when bom[1] == 0xbb && bom[2] == 0xbf:
                    return EncodingInfos.CreateNew(Encoding.UTF8, true);
                case 0xff when bom[1] == 0xfe:
                    return EncodingInfos.CreateNew(Encoding.Unicode, true); //UTF-16LE
                case 0xfe when bom[1] == 0xff:
                    return EncodingInfos.CreateNew(Encoding.BigEndianUnicode, true); //UTF-16BE
                case 0 when bom[1] == 0 && bom[2] == 0xfe:
                    return EncodingInfos.CreateNew(Encoding.UTF32, true);
                default:
                {
                    sourceFile.Position = 0;
                    return EncodingInfos.CreateNew(Encoding.Default, false);
                }
            }
        }
    }
}

