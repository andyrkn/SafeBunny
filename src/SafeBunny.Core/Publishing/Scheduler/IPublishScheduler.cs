using System;
using System.Threading.Tasks;

namespace SafeBunny.Core.Publishing.Scheduler
{
    internal interface IPublishScheduler
    {
        public Task ScheduleAsync(Func<Task> func);
    }
}