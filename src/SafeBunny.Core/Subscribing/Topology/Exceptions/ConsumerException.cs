using System;

namespace SafeBunny.Core.Subscribing.Topology.Exceptions
{
    public sealed class ConsumerException : Exception
    {
        public ConsumerException(string err, Exception innerException) : base(err, innerException)
        { }

        public ConsumerException(string err) : base(err)
        { }
    }
}