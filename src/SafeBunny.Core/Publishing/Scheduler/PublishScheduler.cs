using System;
using System.Threading;
using System.Threading.Tasks;

namespace SafeBunny.Core.Publishing.Scheduler
{
    internal sealed class PublishScheduler : IPublishScheduler
    {
        private readonly TaskScheduler _scheduler = 
            new ConcurrentExclusiveSchedulerPair(TaskScheduler.Default, 8, 1)
                .ExclusiveScheduler;

        public async Task ScheduleAsync(Func<Task> func)
            => await (await Task.Factory
                    .StartNew(func, CancellationToken.None,
                    TaskCreationOptions.LongRunning | TaskCreationOptions.RunContinuationsAsynchronously,
                    _scheduler)
                    .ConfigureAwait(false))
                .ConfigureAwait(false);
    }
}