using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SafeBunny.Core.Subscribing.Transactional;
using SafeBunny.CosmosDbStore.ClientFactory;
using SafeBunny.CosmosDbStore.Transactional;

namespace SafeBunny.CosmosDbStore
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddSafeBunnyCosmosStore(this IServiceCollection services,
            IConfiguration config)
            => services
                .Configure<SafeBunnyCosmosSettings>(o => config.GetSection(nameof(SafeBunnyCosmosSettings)).Bind(o))
                .AddSingleton<ISafeBunnyCosmosClientFactory, SafeBunnyCosmosClientFactory>()
                .AddSingleton<SafeBunnyCosmosClient>()
                .AddScoped<ITransactionalScope, TransactionalScope>()
                .AddScoped<ITransactionalStore, CosmosDbTransactionalStore>();
    }
}