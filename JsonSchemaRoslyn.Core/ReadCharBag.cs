using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using JetBrains.Annotations;

namespace JsonSchemaRoslyn.Core
{
    /// <summary>
    /// Collection of read chars
    /// </summary>
    public sealed class ReadCharBag : IDisposable, IEnumerable<Char>
    {
        [NotNull] private Char[] _readChars = new Char[0];

        [NotNull] private static readonly object _locker = new object();

        public static ReadCharBag FromCharBag([NotNull]ReadCharBag initialBag)
        {
            if (initialBag == null) throw new ArgumentNullException(nameof(initialBag));
            var result = new ReadCharBag {_readChars = new char[initialBag._readChars.Length]};

            initialBag._readChars.CopyTo(result._readChars, 0);

            return result;
        }

        public void Add(Char @byte)
        {
            lock (_locker)
            {
                int newLength = _readChars.Length + 1;
                char[] newArray = new char[newLength];
                _readChars.CopyTo(newArray, 0);
                newArray[newLength - 1] = @byte;

                _readChars = newArray;
            }
        }

        /// <inheritdoc />
        public void Dispose()
        {
            EmptyBag();
        }

        /// <inheritdoc />
        public IEnumerator<char> GetEnumerator()
        {
            lock (_locker)
            {
                return _readChars.AsEnumerable().GetEnumerator();
            }
        }

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator()
        {
            lock (_locker)
            {
                return _readChars.GetEnumerator();
            }
        }

        public static implicit operator Char[](ReadCharBag c)
        {
            lock (_locker)
            {
                return c?._readChars ?? new char[0];
            }
        }

        public static implicit operator Byte[](ReadCharBag c)
        {
            lock (_locker)
            {
                return c?._readChars.Select(Convert.ToByte).ToArray() ?? new Byte[0];
            }
        }

        /// <inheritdoc />
        public override string ToString()
        {
            string result;
            lock (_locker)
            {
                result = new String(_readChars);
            }
            Debug.Assert(result != "\uffff");
            return result;
        }

        public void EmptyBag()
        {
            lock (_locker)
            {
                _readChars = new Char[0];
            }
        }
    }
}