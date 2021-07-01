using System;

namespace SafeBunny.Core.Publishing.Continuation
{
    internal sealed class CorrelationContinuation : ICorrelationContinuation
    {
        private const uint _start = 97;
        private const uint _limit = 25;

        public string Continue(string correlationId, uint continuationMarker)
        {
            var continuation = new Span<char>(correlationId.ToCharArray());

            var length = continuation.Length - 1;
            if (continuationMarker == 0)
            {
                continuation[length] = 'a';
                return continuation.ToString();
            }

            var index = 0;
            var temp = continuationMarker;
            while(temp > 0)
            {
                var remainder = (int) (temp % _limit);
                continuation[length - index++] = ToChar(remainder);
                temp /= _limit;
            }

            return continuation.ToString();
        }

        private static char ToChar(int i) => (char) (_start + i);
    }
}