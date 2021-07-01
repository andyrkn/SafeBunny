using System.Net;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Options;
using SafeBunny.Core;
using SafeBunny.CosmosDbStore.Exceptions;
using SafeBunny.CosmosDbStore.Transactional;

namespace SafeBunny.CosmosDbStore.ClientFactory
{
    internal sealed class SafeBunnyCosmosClientFactory : ISafeBunnyCosmosClientFactory
    {
        private readonly SafeBunnyCosmosSettings _cosmosSettings;
        private readonly SafeBunnySettings _settings;

        public SafeBunnyCosmosClientFactory(IOptions<SafeBunnyCosmosSettings> cosmosSettings, IOptions<SafeBunnySettings> settings)
        {
            _cosmosSettings = cosmosSettings.Value;
            _settings = settings.Value;
        }

        public async Task<CosmosClient> GetClient()
        {

            var client = new CosmosClient(_cosmosSettings.EndpointUrl, _cosmosSettings.Key,new CosmosClientOptions
            {
                SerializerOptions = new CosmosSerializationOptions
                {
                    PropertyNamingPolicy = CosmosPropertyNamingPolicy.CamelCase
                }
            });

            var database = await SetupDatabase(client);
            await SetupContainer(database);

            return client;
        }

        private async Task<Database> SetupDatabase(CosmosClient client)
        {
            var response = await client.CreateDatabaseIfNotExistsAsync(_cosmosSettings.DatabaseId);

            if (response.StatusCode != HttpStatusCode.Created &&
                response.StatusCode != HttpStatusCode.OK)
            {
                throw new SafeBunnyCosmosException("Unable to create cosmos database");
            }

            return response.Database;
        }

        private async Task SetupContainer(Database database)
        {
            var response = await database.CreateContainerIfNotExistsAsync(new ContainerProperties
            {
                Id = _settings.Node,
                PartitionKeyPath = ConstantKeys.CorrelationId,
                UniqueKeyPolicy = new UniqueKeyPolicy
                {
                    UniqueKeys =
                    {
                        new UniqueKey
                        {
                            Paths =
                            {
                                ConstantKeys.MessageId
                            }
                        }
                    }
                }
            });

            if (response.StatusCode != HttpStatusCode.Created &&
                response.StatusCode != HttpStatusCode.OK)
            {
                throw new SafeBunnyCosmosException($"Unable to create cosmos container for node {_settings.Node} {response.Diagnostics} ");
            }
        }
    }
}