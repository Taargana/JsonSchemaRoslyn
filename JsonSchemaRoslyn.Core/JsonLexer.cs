using System;
using System.ComponentModel;
using System.IO;
using System.Numerics;
using System.Text;
using JetBrains.Annotations;
using JsonSchemaRoslyn.Core.Exceptions;
using JsonSchemaRoslyn.Core.Extensions;

namespace JsonSchemaRoslyn.Core
{
    /// <summary>
    /// convert a sequence of characters into a sequence of tokens
    /// </summary>
    public sealed class JsonLexer : ILexer
    {
        [NotNull] private Stream _sourceStream;
        private long _position = 0;
        private EncodingInfos _detectEncodingFromByteOrderMarks;
        [NotNull] private readonly ReadCharBag _readCharBag;

        [NotNull]
        public DiagnosticBag Diagnostics { get; }

        public JsonLexer([NotNull] Stream sourceStream, Encoding encoding = null) : this()
        {
            if (sourceStream == null || sourceStream == Stream.Null)
            {
                throw new ArgumentNullException(nameof(sourceStream));
            }

            _sourceStream = sourceStream;

            _detectEncodingFromByteOrderMarks = _sourceStream.GetEncoding();
        }

        public JsonLexer([NotNull] FileInfo sourceFile) : this()
        {
            if (sourceFile == null) throw new ArgumentNullException(nameof(sourceFile));

            CreateSourceStream(sourceFile);
        }

        public JsonLexer([NotNull] string path) : this()
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentException($"The path is null or white space", nameof(path));
            }

            CreateSourceStream(new FileInfo(path));
        }

        private JsonLexer()
        {
            _readCharBag = new ReadCharBag();
            Diagnostics = new DiagnosticBag();
        }

        private void CreateSourceStream([NotNull] FileInfo sourceFile)
        {
            if (sourceFile == null) throw new ArgumentNullException(nameof(sourceFile));
            if (!sourceFile.Exists)
            {
                throw new FileNotFoundException($"The file {sourceFile.FullName} does not exist");
            }

            _sourceStream = new FileStream(sourceFile.FullName, FileMode.Open, FileAccess.Read);

            _detectEncodingFromByteOrderMarks = _sourceStream.GetEncoding();

            if (_detectEncodingFromByteOrderMarks == null)
            {
                throw new EncodingInfosNullException();
            }

            _position = _sourceStream.Position;
        }

        /// <inheritdoc />
        public SyntaxToken Lex()
        {
            object value = null;
            SyntaxKind kind = SyntaxKind.Unknown;
            string text = String.Empty;
            long startPosition = _position;

            using (_readCharBag)
            {
                int readByte = _sourceStream.ReadByte();
                if (readByte == -1)
                {
                    kind = SyntaxKind.EndOfFile;
                }
                else
                {
                    char currentChar = (char) readByte;
                    _readCharBag.Add(currentChar);
                    switch (currentChar)
                    {
                        case '\0':
                            kind = SyntaxKind.EndOfFile;
                            break;
                        case '{':
                            kind = SyntaxKind.CurlyBracketLeft;
                            break;
                        case '}':
                            kind = SyntaxKind.CurlyBracketRight;
                            break;
                        case '"':
                            kind = SyntaxKind.DoubleQuote;
                            break;
                        case '\'':
                            kind = SyntaxKind.SimpleQuote;
                            break;
                        case '-':
                            kind = SyntaxKind.Minus;
                            break;
                        case ',':
                            kind = SyntaxKind.Coma;
                            break;
                        case ';':
                            kind = SyntaxKind.SemiColon;
                            break;
                        case '=':
                            kind = SyntaxKind.Equal;
                            break;
                        case '/':
                            kind = SyntaxKind.Slash;
                            break;
                        case '\\':
                            kind = SyntaxKind.BackSlash;
                            break;
                        case '*':
                            kind = SyntaxKind.Star;
                            break;
                        case ':':
                            kind = SyntaxKind.Colon;
                            break;
                        case '.':
                            kind = SyntaxKind.Dot;
                            break;
                        case '+':
                            kind = SyntaxKind.Plus;
                            break;
                        case ' ':
                        case '\t':
                        case '\n':
                        case '\r':
                            ExtractWhisteSpace();
                            kind = SyntaxKind.Whitespace;
                            break;
                        default:
                            if (char.IsLetter(currentChar))
                            {
                                ExtractLiteral();
                                kind = SyntaxKind.Letter;
                            }
                            else if (char.IsDigit(currentChar))
                            {
                                ExtractDigit();
                                value = BigInteger.Parse(_readCharBag.ToString());
                                kind = SyntaxKind.Digit;
                            }
                            break;
                    }

                    text = _readCharBag.ToString();
                    if (kind == SyntaxKind.Unknown || kind == SyntaxKind.Count)
                    {
                        Diagnostics.AddDiagnostic(new Diagnostic(new TextSpan(startPosition, text?.Length ?? 0),
                            $"Unknown character(s) {text}"));
                    }
                }
            }
            return new SyntaxToken(kind, text, startPosition, value);
        }

        private void ExtractWhisteSpace()
        {
            char tmpWhiteSpace;
            do
            {
                tmpWhiteSpace = (char) _sourceStream.ReadByte();

                if (char.IsWhiteSpace(tmpWhiteSpace))
                {
                    _readCharBag.Add(tmpWhiteSpace);
                }
            } while (char.IsWhiteSpace(tmpWhiteSpace));
            _sourceStream.Position = _sourceStream.Position - 1;
        }

        private void ExtractLiteral()
        {
            char tmpLetter;
            do
            {
                tmpLetter = (char) _sourceStream.ReadByte();

                if (char.IsLetter(tmpLetter))
                {
                    _readCharBag.Add(tmpLetter);
                }
            } while (char.IsLetter(tmpLetter));
            _sourceStream.Position = _sourceStream.Position - 1;
        }

        private void ExtractDigit()
        {
            char tmpLetter;
            do
            {
                tmpLetter = (char) _sourceStream.ReadByte();

                if (char.IsDigit(tmpLetter))
                {
                    _readCharBag.Add(tmpLetter);
                }
            } while (char.IsDigit(tmpLetter));
            _sourceStream.Position = _sourceStream.Position - 1;
        }

        /// <inheritdoc />
        public void Dispose()
        {
            _sourceStream.Dispose();
        }
    }
}