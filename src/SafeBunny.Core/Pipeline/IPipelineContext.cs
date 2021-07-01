using System;

namespace SafeBunny.Core.Pipeline
{
    public interface IPipelineContext
    {
        internal IServiceProvider Provider { get; set; }
    }
}