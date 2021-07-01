using Newtonsoft.Json;
using SafeBunny.Core.Subscribing.Transactional;

namespace SafeBunny.CosmosDbStore.Transactional
{
    internal sealed class CosmosTransactionalState
    {
        public CosmosTransactionalState(){}

        public CosmosTransactionalState(TransactionalState state)
        {
            this.CorrelationId = state.CorrelationId;
            this.MessageId = state.MessageId;
        }

        [JsonProperty("id")] public string Id => $"{CorrelationId}{MessageId}";

        [JsonProperty("correlationId")]
        public string CorrelationId { get; set; }

        [JsonProperty("messageId")]
        public string MessageId { get; set; }
    }
}