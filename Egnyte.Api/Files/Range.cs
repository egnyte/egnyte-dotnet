namespace Egnyte.Api.Files
{
    using System;

    public class Range
    {
        public Range(int from, int to)
        {
            if (from > to)
            {
                throw new ArgumentOutOfRangeException("from", "'From' parameter must be less or equal to 'to'");
            }

            From = from;
            To = to;
        }

        public int From { get; private set; }

        public int To { get; private set; }
    }
}
