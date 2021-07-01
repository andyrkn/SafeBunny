using System;

namespace SafeBunny.CosmosDbStore.Exceptions
{
    public sealed class SafeBunnyCosmosException : Exception
    {
        public SafeBunnyCosmosException(string err) : base(err)
        { }
    }
}