using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;

namespace SafeBunny.CosmosDbStore.ClientFactory
{
    internal interface ISafeBunnyCosmosClientFactory
    {
        Task<CosmosClient> GetClient();
    }
}