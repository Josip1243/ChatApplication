using ChatApplicationServer.Models;

namespace ChatApplicationServer.Repository
{
    public interface IConnectionsRepository
    {
        void AddConnection(Connection connection);
        List<Connection> GetConnections();
        void RemoveConnection(Connection connection);
    }
}