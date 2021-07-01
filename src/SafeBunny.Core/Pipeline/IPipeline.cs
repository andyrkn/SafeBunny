using System.Threading.Tasks;

namespace SafeBunny.Core.Pipeline
{
    internal interface IPipeline<TPipelineContext> where TPipelineContext: IPipelineContext
    {
        void RegisterCoreProcessor<T>() where T : ISafeBunnyMiddleware<TPipelineContext>;
        void RegisterPreProcessor<T>() where T : ISafeBunnyMiddleware<TPipelineContext>;
        void RegisterPostProcessor<T>() where T : ISafeBunnyMiddleware<TPipelineContext>;

        Task ProcessAsync(TPipelineContext context);
    }
}