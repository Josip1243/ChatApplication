using ChatApplicationServer.Models;
using ChatApplicationServer.Repository;
using System.Collections.Specialized;
using System.ComponentModel;

namespace ChatApplicationServer.Services
{
    public class ConnectionService : IConnectionService
    {
        // Uncoment when using DB
        //private ChatContext context;

        // For testing only (before DB setup)
        private ConnectionsRepositoryMock connectionRepository;

        public ConnectionService(/*ChatContext context,*/ ConnectionsRepositoryMock connectionRepository)
        {
            //this.context = context;
            this.connectionRepository = connectionRepository;
        }

        public async void AddConnection(Connection connection)
        {
            connectionRepository.AddConnection(connection);

            // Uncomment when using DB
            //await context.Connections.AddAsync(connection);
            //await context.SaveChangesAsync();
        }

        public async void RemoveConnection(string connectionId)
        {
            var tempConn = connectionRepository.GetConnections().FirstOrDefault(conn => conn.SignalRId == connectionId);
            connectionRepository.RemoveConnection(tempConn);
        }

        public async Task<Connection> GetConnection(string connectionId)
        {
            return connectionRepository.GetConnections().FirstOrDefault(conn => conn.SignalRId == connectionId);
        }

        public List<Connection> GetConnections()
        {
            return connectionRepository.GetConnections();
        }
    }
}
