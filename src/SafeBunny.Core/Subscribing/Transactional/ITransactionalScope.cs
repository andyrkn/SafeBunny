using System;
using System.Threading.Tasks;

namespace SafeBunny.Core.Subscribing.Transactional
{
    internal interface ITransactionalScope
    {
        Task CommitAsync(string correlationId, string messageId, Func<Task> transaction);
    }
}