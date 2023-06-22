using ChatApplicationServer.Models;
using ChatApplicationServer.Repository;
using System.Collections.Specialized;
using System.ComponentModel;

namespace ChatApplicationServer.Services
{
    public class ConnectionService // : IConnectionService
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

        public async void AddConnection(int userId, string signalrConnectionId)
        {
            var newConnection = new Connection() { UserId = userId, SignalRId = signalrConnectionId, timeStamp = DateTime.Now };

            connectionRepository.AddConnection(newConnection);

            // Uncomment when using DB
            //await context.Connections.AddAsync(connection);
            //await context.SaveChangesAsync();
        }

        public async void RemoveConnection(string connectionId)
        {
            var tempConn = connectionRepository.GetConnections().FirstOrDefault(conn => conn.SignalRId == connectionId);
            connectionRepository.RemoveConnection(tempConn);
        }

        public async Task<Connection> GetConnection(int userId, string connectionId)
        {
            return connectionRepository.GetConnections().FirstOrDefault(conn => conn.UserId == userId && conn.SignalRId == connectionId);
        }

        public List<Connection> GetConnections()
        {
            return connectionRepository.GetConnections();
        }
        public List<Connection> GetConnections(IEnumerable<int> userIds)
        {
            return connectionRepository.GetConnections().Where(conn => userIds.Contains(conn.UserId)).ToList();
        }
    }
}
