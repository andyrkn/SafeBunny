using System;
using System.Threading.Tasks;

namespace SafeBunny.Core.Subscribing.Transactional
{
    internal sealed class TransactionalScope : ITransactionalScope
    {
        private readonly ITransactionalStore _store;

        public TransactionalScope(ITransactionalStore store)
        {
            _store = store;
        }

        public async Task CommitAsync(string correlationId, string messageId, Func<Task> transaction)
        {
            var state = new TransactionalState(correlationId, messageId);

            if (await _store.CheckAsync(state))
            {
                await transaction();
                await _store.SaveAsync(state);
            }
        }
    }
}