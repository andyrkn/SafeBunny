using System;

namespace SafeBunny.Core.Extensions.Exceptions
{
    public sealed class ConfigurationException : Exception
    {
        public ConfigurationException(string err) : base(err)
        { }
    }
}