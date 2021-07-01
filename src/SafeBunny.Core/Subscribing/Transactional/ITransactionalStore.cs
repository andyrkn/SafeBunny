using System.Threading.Tasks;

namespace SafeBunny.Core.Subscribing.Transactional
{
    internal interface ITransactionalStore
    {
        Task SaveAsync(TransactionalState state);

        Task<bool> CheckAsync(TransactionalState state);
    }
}