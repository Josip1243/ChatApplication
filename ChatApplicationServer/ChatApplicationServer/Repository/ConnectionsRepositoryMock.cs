using ChatApplicationServer.Models;

namespace ChatApplicationServer.Repository
{
    public class ConnectionsRepositoryMock
    {
        List<Connection> connections;

        public ConnectionsRepositoryMock()
        {
            connections = new List<Connection>();
        }

        public void AddConnection(Connection connection)
        {
            connections.Add(connection);
        }

        public List<Connection> GetConnections()
        {
            return connections;
        }

        public void RemoveConnection(Connection connection)
        {
            connections.Remove(connection);
        }
    }
}
