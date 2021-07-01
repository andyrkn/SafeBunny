using Microsoft.Extensions.Options;
using SafeBunny.Core.Subscribing.Topology;

namespace SafeBunny.Core.Subscribing.Builder
{
    internal sealed class SubscriptionManager : ISubscriptionManager
    {
        private readonly ITopology _topology;
        private readonly SafeBunnySettings _settings;

        public SubscriptionManager(ITopology topology, IOptions<SafeBunnySettings> settings)
        {
            _topology = topology;
            _settings = settings.Value;
        }


        public ISubscriptionBuilder FromNode(string node)
        {
            _topology.Register(node);
            return new SubscriptionBuilder(node, _topology, _settings.BasicQoS);
        }
    }
}