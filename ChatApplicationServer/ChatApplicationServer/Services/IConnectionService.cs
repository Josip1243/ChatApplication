using ChatApplicationServer.Models;

namespace ChatApplicationServer.Services
{
    public interface IConnectionService
    {
        void AddConnection(int userId, string signalrConnectionId);
        Task<Connection> GetConnection(int userId, string connectionId);
        List<Connection> GetConnections();
        List<Connection> GetConnections(IEnumerable<int> userIds);
        void RemoveConnection(string connectionId);
    }
}