using ChatApplicationServer.Models;
using System;

namespace ChatApplicationServer.Repository
{
    public class ConnectionsRepository : IConnectionsRepository
    {
        private ChatAppContext _appContext;

        public ConnectionsRepository(ChatAppContext appContext)
        {
            _appContext = appContext;
        }

        public void AddConnection(Connection connection)
        {
            _appContext.Connections.Add(connection);
            _appContext.SaveChanges();
        }

        public List<Connection> GetConnections()
        {
            return _appContext.Connections.ToList();
        }

        public void RemoveConnection(Connection connection)
        {
            if (connection != null)
            {
                _appContext.Connections.Remove(connection);
                _appContext.SaveChanges();
            }
        }
    }
}
