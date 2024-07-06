namespace Egnyte.Api.Files
{
    using System;

    public class Range
    {
        public Range(long from, long to)
        {
            if (from > to)
            {
                throw new ArgumentOutOfRangeException(nameof(from), "'From' parameter must be less or equal to 'to'");
            }

            From = from;
            To = to;
        }

        public long From { get; private set; }

        public long To { get; private set; }
    }
}
