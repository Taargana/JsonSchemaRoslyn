using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
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
        /// <summary>
        /// true when the previous char was "; false otherwise
        /// </summary>
        [NotNull] private readonly ReadCharBag _readCharBag;
        private char _currentChar;

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

        public JsonLexer([NotNull] string content) : this()
        {
            if (string.IsNullOrWhiteSpace(content))
            {
                throw new ArgumentException($"The content should not be empty or null", nameof(content));
            }

            _sourceStream = new MemoryStream();
            var writer = new StreamWriter(_sourceStream);
            writer.Write(content);
            writer.Flush();
            _sourceStream.Position = 0;
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
        public IEnumerable<SyntaxToken> Lex()
        {
            object value = null;
            SyntaxKind kind = SyntaxKind.Unknown;
            string text = String.Empty;
            long startPosition = _position;

            do
            {
                using (_readCharBag)
                {
                    int readByte = _sourceStream.ReadByte();
                    if (readByte == -1)
                    {
                        kind = SyntaxKind.EndOfFile;
                    }
                    else
                    {
                        _currentChar = (char)readByte;
                        _readCharBag.Add(_currentChar);

                        switch (_currentChar)
                        {
                            case '\0':
                                kind = SyntaxKind.EndOfFile;
                                break;
                            case '{':
                                kind = SyntaxKind.OpenObjectCurlyBracket;
                                break;
                            case '}':
                                kind = SyntaxKind.CloseObjectCurlyBracket;
                                break;
                            case '\'':
                            case '"':
                                _readCharBag.EmptyBag();
                                ExtractLiteral();
                                kind = SyntaxKind.Literal;
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
                                if (char.IsLetter(_currentChar))
                                {
                                    ExtractLiteral();
                                    kind = SyntaxKind.PropertyName;
                                }
                                else if (char.IsDigit(_currentChar))
                                {
                                    ExtractDigit();
                                    value = BigInteger.Parse(_readCharBag.ToString());
                                    kind = SyntaxKind.Digit;
                                }
                                else
                                {
                                    kind = SyntaxKind.Unknown;
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

                yield return new SyntaxToken(kind, text, startPosition, value);
            } while (kind != SyntaxKind.EndOfFile);
        }

        private void ExtractWhisteSpace()
        {
            do
            {
                _currentChar = (char)_sourceStream.ReadByte();
                if (char.IsWhiteSpace(_currentChar))
                {
                    _readCharBag.Add(_currentChar);
                }
            } while (char.IsWhiteSpace(_currentChar));

            _sourceStream.Position--;
        }

        private void ExtractLiteral(ReadCharBag charBag = null)
        {
            var currentBag = charBag ?? _readCharBag;
            char previousChar = default(char);

            while (true)
            {
                _currentChar = (char)_sourceStream.ReadByte();
                if (_currentChar == '"' && previousChar != '\\')
                {
                    break;
                }
                currentBag.Add(_currentChar);
                previousChar = _currentChar;
            }
        }

        private void ExtractDigit()
        {
            do
            {
                _currentChar = (char)_sourceStream.ReadByte();
                if (char.IsDigit(_currentChar))
                {
                    _readCharBag.Add(_currentChar);
                }
            } while (char.IsDigit(_currentChar));
            _sourceStream.Position--;
        }

        /// <inheritdoc />
        public void Dispose()
        {
            _sourceStream.Dispose();
        }
    }
}