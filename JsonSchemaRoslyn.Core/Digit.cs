using System;
using System.Globalization;

namespace JsonSchemaRoslyn.Core
{
    [Serializable]
    public struct Digit : IComparable, IFormattable, IConvertible, IComparable<Digit>, IEquatable<Digit>
    {
        private byte m_value;
        private const byte minvalue = 0b0;
        private const byte maxvalue = 0b1001;
        public const Digit MaxValue = maxvalue;
        public const Digit MinValue = minvalue;

        public int CompareTo(object value)
        {
            if (value == null)
                return 1;
            if (!(value is Digit))
                throw new ArgumentException("Must be a Digit");
            Digit num = (Digit)value;
            if (this < num)
                return -1;
            return this > num ? 1 : 0;
        }

        public int CompareTo(Digit value)
        {
            if (this < value)
                return -1;
            return this > value ? 1 : 0;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Digit))
                return false;
            return this == (Digit)obj;
        }

        public bool Equals(Digit obj)
        {
            return this == obj;
        }

        public override int GetHashCode()
        {
            return this;
        }

        public override string ToString()
        {
            return m_value.ToString();
        }

        public string ToString(string format)
        {
            return m_value.ToString(format, NumberFormatInfo.CurrentInfo);
        }

        public string ToString(IFormatProvider provider)
        {
            return m_value.ToString(NumberFormatInfo.GetInstance(provider));
        }

        public string ToString(string format, IFormatProvider provider)
        {
            return m_value.ToString(format, NumberFormatInfo.GetInstance(provider));
        }

        public static Digit Parse(string s)
        {
            return Parse(s, NumberStyles.Number);
        }

        public static Digit Parse(string s, NumberStyles style)
        {
            if (Byte.TryParse(s, style, NumberFormatInfo.CurrentInfo, out Byte @byte) && @byte < maxvalue && @byte > minvalue)
            {
                return new Digit
                {
                    m_value = @byte
                };
            }
            throw new Exception("Cannot parse string to Digit");
        }

        /*public static Digit Parse(string s, IFormatProvider provider)
        {
            return Number.ParseDigit32(s, NumberStyles.Digiteger, NumberFormatInfo.GetInstance(provider));
        }

        public static Digit Parse(string s, NumberStyles style, IFormatProvider provider)
        {
            NumberFormatInfo.ValidateParseStyleDigiteger(style);
            return Number.ParseDigit32(s, style, NumberFormatInfo.GetInstance(provider));
        }

        public static bool TryParse(string s, out Digit result)
        {
            return Number.TryParseDigit32(s, NumberStyles.Digiteger, NumberFormatInfo.CurrentInfo, out result);
        }

        public static bool TryParse(
            string s,
            NumberStyles style,
            IFormatProvider provider,
            out Digit result)
        {
            NumberFormatInfo.ValidateParseStyleDigiteger(style);
            return Number.TryParseDigit32(s, style, NumberFormatInfo.GetInstance(provider), out result);
        }*/

        public TypeCode GetTypeCode()
        {
            return TypeCode.Byte;
        }

        public static implicit operator Byte(Digit d)
        {
            return d.m_value;
        }

        public static implicit operator Digit(int d)
        {
            byte mValue = Convert.ToByte(d);

            if (mValue < minvalue || mValue > maxvalue)
            {
                throw new Exception("Cannot convert int to Digit. The int must be between 0 and 9");
            }

            return new Digit
            {
                m_value = mValue;
            };
        }

        bool IConvertible.ToBoolean(IFormatProvider provider)
        {
            return Convert.ToBoolean(this);
        }

        char IConvertible.ToChar(IFormatProvider provider)
        {
            return Convert.ToChar(this);
        }

        sbyte IConvertible.ToSByte(IFormatProvider provider)
        {
            return Convert.ToSByte(this);
        }

        byte IConvertible.ToByte(IFormatProvider provider)
        {
            return Convert.ToByte(this);
        }

        short IConvertible.ToInt16(IFormatProvider provider)
        {
            return Convert.ToInt16(this);
        }

        ushort IConvertible.ToUInt16(IFormatProvider provider)
        {
            return Convert.ToUInt16(this);
        }

        int IConvertible.ToInt32(IFormatProvider provider)
        {
            return Convert.ToInt32(value: this);
        }

        uint IConvertible.ToUInt32(IFormatProvider provider)
        {
            return Convert.ToUInt32(this);
        }

        long IConvertible.ToInt64(IFormatProvider provider)
        {
            return Convert.ToInt64(this);
        }

        ulong IConvertible.ToUInt64(IFormatProvider provider)
        {
            return Convert.ToUInt64(this);
        }

        float IConvertible.ToSingle(IFormatProvider provider)
        {
            return Convert.ToSingle(this);
        }

        double IConvertible.ToDouble(IFormatProvider provider)
        {
            return Convert.ToDouble(this);
        }

        Decimal IConvertible.ToDecimal(IFormatProvider provider)
        {
            return Convert.ToDecimal(this);
        }

        DateTime IConvertible.ToDateTime(IFormatProvider provider)
        {
            throw new InvalidCastException("Cannot cast Digit to DateTime"));
        }

        object IConvertible.ToType(Type type, IFormatProvider provider)
        {
            //TODO il faut le faire!
            //return Convert.DefaultToType((IConvertible)this, type, provider);
            return null;
        }
    }
}