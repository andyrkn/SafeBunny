using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace SafeBunny.SignalR
{
    public static class Extensions
    {
        public static IServiceCollection AddSignalRClient(
            this IServiceCollection services, IConfiguration configuration)
            => services
                .Configure<SignalRConfig>(b => configuration.GetSection(nameof(SignalRConfig)).Bind(b))
                .AddSingleton<ISignalRClient, SignalRClient>();
    }
}