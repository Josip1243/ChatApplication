using ChatApplicationServer.Models;
using ChatApplicationServer.Models2;
using System;

namespace ChatApplicationServer.Repository
{
    public class ConnectionsRepositoryMock
    {
        private ChatAppContext _appContext;

        public ConnectionsRepositoryMock(ChatAppContext appContext)
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
