using ChatApplicationServer.Models;
using ChatApplicationServer.Models2;

namespace ChatApplicationServer.Services
{
    public interface IConnectionService
    {
        void AddConnection(Connection connection);

        void RemoveConnection(string connectionId);
        Task<Connection> GetConnection(string connectionId);
        List<Connection> GetConnections();
    }
}
