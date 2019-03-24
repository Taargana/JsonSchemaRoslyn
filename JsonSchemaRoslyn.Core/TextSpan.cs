namespace JsonSchemaRoslyn.Core
{
    public struct TextSpan
    {
        public TextSpan(long start, int length)
        {
            Start = start;
            Length = length;
        }

        public long Start { get; }
        public int Length { get; }
        public long End => Start + Length;

        public static TextSpan FromBounds(int start, int end)
        {
            int length = end - start;
            return new TextSpan(start, length);
        }

        public override string ToString() => $"{Start}..{End}";
    }
}