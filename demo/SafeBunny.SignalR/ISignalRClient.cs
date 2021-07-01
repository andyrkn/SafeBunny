using System.Threading.Tasks;

namespace SafeBunny.SignalR
{
    public interface ISignalRClient
    {
        Task Subscribe();
        Task NotifyTransaction(string data);
        Task NotifyEvent(string data);
        Task NotifyException(string data);
    }
}