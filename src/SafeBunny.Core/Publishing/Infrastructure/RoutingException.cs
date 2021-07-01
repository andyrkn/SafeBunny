using System;

namespace SafeBunny.Core.Publishing.Infrastructure
{
    public sealed class RoutingException : Exception
    {
        public RoutingException(string error) : base(error)
        {
            
        }
    }
}