using System;
using System.Threading.Tasks;
using SafeBunny.Core.Pipeline;
using SafeBunny.Core.Publishing.Pipeline;

namespace SafeBunny.Consumer.Orders
{
    public class ChaosTheoryPublisher : ISafeBunnyMiddleware<IPublishingContext>
    {
        public async Task InvokeAsync(IPublishingContext context, Func<Task> next)
        {
            await next();
            await next();
        }
    }
}