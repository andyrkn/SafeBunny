namespace SafeBunny.Core.Subscribing.Transactional
{
    internal class TransactionalState
    {
        public TransactionalState(string correlationId, string messageId)
        {
            CorrelationId = correlationId;
            MessageId = messageId;
        }

        public string CorrelationId { get; }
        public string MessageId { get; }
    }
}