using Microsoft.Azure.Cosmos;

namespace SafeBunny.CosmosDbStore.Transactional
{
    internal static class ConstantKeys
    {
        internal const string CorrelationId = "/correlationId";
        internal const string MessageId = "/messageId";
        internal static readonly PartitionKey PartitionKey = new(CorrelationId);
    }
}