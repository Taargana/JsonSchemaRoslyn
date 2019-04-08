using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using JetBrains.Annotations;
using JsonSchemaRoslyn.Core.Arrays;
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
            if (content==null)
            {
                throw new ArgumentException($"The content should not be empty or null", nameof(content));
            }

            _sourceStream = new MemoryStream();
            StreamWriter writer = new StreamWriter(_sourceStream);
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
            SyntaxKind kind;
            do
            {
                object value = null;
                string text = null;
                kind = SyntaxKind.Unknown;
                long startPosition = _sourceStream.Position;

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
                            case '\uffff':
                            case '\0':
                                kind = SyntaxKind.EndOfFile;
                                break;
                            case '{':
                                kind = SyntaxKind.OpenObjectCurlyBracket;
                                break;
                            case '}':
                                kind = SyntaxKind.CloseObjectCurlyBracket;
                                break;
                            case '[':
                                kind = SyntaxKind.OpenArrayBracket;
                                break;
                            case ']':
                                kind = SyntaxKind.CloseArrayBracket;
                                break;
                            case '\'':
                            case '"':
                                _readCharBag.EmptyBag();
                                try
                                {
                                    ExtractLiteral();
                                    kind = SyntaxKind.Literal;
                                }
                                catch (EndOfFileExtractLiteralException e)
                                {
                                    text = _readCharBag.ToString();
                                    Diagnostics.AddDiagnostic(new Diagnostic(new TextSpan(startPosition, text?.Length ?? 0), $"The string is open but never closed. {text}", e));
                                    continue;
                                }
                                break;
                            case '-':
                                kind = SyntaxKind.Minus;
                                break;
                            case ',':
                                kind = SyntaxKind.Coma;
                                break;
                            case '/':
                                kind = SyntaxKind.Slash;
                                break;
                            case '\\':
                                kind = SyntaxKind.BackSlash;
                                break;
                            case ':':
                                kind = SyntaxKind.Colon;
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
                                        try
                                        {
                                            ExtractKeyword();
                                        }
                                        catch (EndOfFileExtractLiteralException e)
                                        {
                                            text = _readCharBag.ToString();
                                            Diagnostics.AddDiagnostic(new Diagnostic(new TextSpan(startPosition, text?.Length ?? 0), $"the allowed keywords are boolean values, null and object", e));
                                            continue;
                                        }

                                        string tmpValue = _readCharBag.ToString();

                                        StringComparer currentCultureIgnoreCase = StringComparer.CurrentCultureIgnoreCase;
                                        if (currentCultureIgnoreCase.Compare(tmpValue, "true") == 0 ||
                                            currentCultureIgnoreCase.Compare(tmpValue, "false") == 0)
                                        {
                                            value = currentCultureIgnoreCase.Compare(tmpValue, "true") == 0;
                                            kind = SyntaxKind.Boolean;
                                        }
                                        else if (currentCultureIgnoreCase.Compare(tmpValue, "null") == 0)
                                        {
                                            kind = SyntaxKind.Null;
                                        }
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
                                $"Unknown character(s) {text}", new UnknownCharacterException()));
                        }
                    }
                }

                yield return new SyntaxToken(kind, text, startPosition, value);
            } while (kind != SyntaxKind.EndOfFile);
        }

        private void ExtractWhisteSpace(ReadCharBag charBag = null)
        {
            ReadCharBag currentBag = charBag ?? _readCharBag;
            while (true)
            {
                _currentChar = (char)_sourceStream.ReadByte();
                if (!char.IsWhiteSpace(_currentChar))
                {
                    if (_currentChar != '\uffff')
                    {
                        _sourceStream.Position--;
                    }
                    break;
                }
                currentBag.Add(_currentChar);
            }
        }

        private void ExtractLiteral(ReadCharBag charBag = null)
        {
            ReadCharBag currentBag = charBag ?? _readCharBag;
            History<char> previousCharCircularArray = new History<char>(3);
            while (true)
            {
                _currentChar = (char)_sourceStream.ReadByte();
                if (_currentChar == '"')
                {
                    if (previousCharCircularArray.Current == '\\')
                    {
                        if (previousCharCircularArray.Preceding(1) == '\\')
                        {
                            break;
                        }
                    }
                    else
                    {
                        break;
                    }

                }
                else if (_currentChar == '\uffff')
                {
                    throw new EndOfFileExtractLiteralException();
                }
                currentBag.Add(_currentChar);
                previousCharCircularArray.Add(_currentChar);
            }
        }

        private void ExtractKeyword(ReadCharBag charBag = null)
        {
            ReadCharBag currentBag = charBag ?? _readCharBag;

            while (true)
            {
                _currentChar = (char)_sourceStream.ReadByte();
                if (_currentChar == ':' || _currentChar == ','|| char.IsWhiteSpace(_currentChar))
                {
                    break;
                }

                if (_currentChar == '\uffff')
                {
                    throw new EndOfFileExtractLiteralException();
                }
                currentBag.Add(_currentChar);
            }
        }

        private void ExtractDigit(ReadCharBag charBag = null)
            {
                ReadCharBag currentBag = charBag ?? _readCharBag;
                while (true)
                {
                    _currentChar = (char)_sourceStream.ReadByte();
                    if (!char.IsDigit(_currentChar))
                    {
                        break;
                    }
                    currentBag.Add(_currentChar);
                }
            }

        /// <inheritdoc />
        public void Dispose()
        {
            _sourceStream.Dispose();
        }
    }

    public class Number : IFormattable
    {
        private readonly List<Digit> _integerPart;
        private readonly List<Digit> _decimalPart;

        public Number(bool isNegatif)
        {
            IsNegatif = isNegatif;
        }

        public Boolean IsNegatif { get; }

        public IEnumerable<Digit> IntegerPart => _integerPart.ToArray();

        public IEnumerable<Digit> DecimalPart => _decimalPart.ToArray();

        public void AddIntegerPart(Digit digit)
        {
            _integerPart.Add(digit);
        }

        public void AddDecimalPart(Digit digit)
        {
            _decimalPart.Add(digit);
        }

        public string ToString(string format, IFormatProvider formatProvider)
        {
            throw new NotImplementedException();
        }

        public string ToString(string format)
        {
            throw new NotImplementedException();
        }
    }
}