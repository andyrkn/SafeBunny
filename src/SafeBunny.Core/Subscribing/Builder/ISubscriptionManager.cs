namespace SafeBunny.Core.Subscribing.Builder
{
    public interface ISubscriptionManager
    {
        ISubscriptionBuilder FromNode(string node);
    }
}