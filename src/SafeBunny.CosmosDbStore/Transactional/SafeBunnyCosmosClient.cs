using System;
using Microsoft.Azure.Cosmos;
using SafeBunny.CosmosDbStore.ClientFactory;

namespace SafeBunny.CosmosDbStore.Transactional
{
    internal sealed class SafeBunnyCosmosClient : IDisposable
    {
        private readonly ISafeBunnyCosmosClientFactory _factory;
        private Lazy<CosmosClient> _client { get; }

        public SafeBunnyCosmosClient(ISafeBunnyCosmosClientFactory factory)
        {
            _factory = factory;
            _client = new Lazy<CosmosClient>(() => _factory.GetClient().GetAwaiter().GetResult());
        }

        public CosmosClient Client => _client.Value;

        public void Dispose()
        {
            _client.Value.Dispose();
        }
    }
}