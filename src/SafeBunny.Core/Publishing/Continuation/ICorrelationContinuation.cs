namespace SafeBunny.Core.Publishing.Continuation
{
    internal interface ICorrelationContinuation
    {
        string Continue(string correlationId, uint continuationMarker);
    }
}