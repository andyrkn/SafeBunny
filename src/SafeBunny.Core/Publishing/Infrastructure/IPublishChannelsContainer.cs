using System.Threading.Tasks;

namespace SafeBunny.Core.Publishing.Infrastructure
{
    internal interface IPublishChannelsContainer
    {
        public Task<SafeChannel> Get();
        public void Return(SafeChannel channel);
    }
}