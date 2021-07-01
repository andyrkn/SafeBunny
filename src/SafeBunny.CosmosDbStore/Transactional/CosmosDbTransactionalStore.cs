using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Options;
using SafeBunny.Core;
using SafeBunny.Core.Subscribing.Transactional;

namespace SafeBunny.CosmosDbStore.Transactional
{
    internal sealed class CosmosDbTransactionalStore : ITransactionalStore
    {
        private readonly CosmosClient _client;
        private readonly SafeBunnyCosmosSettings _settings;
        private readonly string _node;

        public CosmosDbTransactionalStore(
            SafeBunnyCosmosClient client, 
            IOptions<SafeBunnyCosmosSettings> cosmosSettings, 
            IOptions<SafeBunnySettings> settings)
        {
            _client = client.Client;
            _settings = cosmosSettings.Value;
            _node = settings.Value.Node;
        }

        public async Task SaveAsync(TransactionalState state)
        {
            try
            {
                var cosmosState = new CosmosTransactionalState(state);
                var response = await _client.GetContainer(_settings.DatabaseId, _node)
                    .CreateItemAsync(cosmosState, new PartitionKey(cosmosState.CorrelationId));
            }
            catch (CosmosException ex)
            {
                Handle(ex);
            }
        }

        public async Task<bool> CheckAsync(TransactionalState state)
        {
            try
            {
                var response = await _client.GetContainer(_settings.DatabaseId, _node)
                    .ReadItemAsync<TransactionalState>($"{state.CorrelationId}{state.MessageId}", new PartitionKey(state.CorrelationId));
                
                return response.Resource == null;
            }
            catch (CosmosException ex)
            {
                if (ex.StatusCode == HttpStatusCode.NotFound)
                {
                    return true;
                }

                Handle(ex);
                throw;
            }
        }

        public void Handle(AggregateException exception)
        {
            var exceptions = exception.InnerExceptions.ToList();
            if (exceptions.Any(e => e is not CosmosException))
            {
                throw exception;
            }

            exceptions.ForEach(e => Handle((CosmosException) e));
        }

        public void Handle(CosmosException ex)
        {
            switch (ex.StatusCode)
            {
                case HttpStatusCode.Conflict:
                    return;
                case HttpStatusCode.BadGateway:
                    throw ex;
            }
        }
    }
}
