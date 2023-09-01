using ChatApplicationServer.Models;
using ChatApplicationServer.Repository;
using System.Collections.Specialized;
using System.ComponentModel;

namespace ChatApplicationServer.Services
{
    public class ConnectionService : IConnectionService
    {
        private IConnectionsRepository _connectionRepository;

        public ConnectionService(IConnectionsRepository connectionRepository)
        {
            this._connectionRepository = connectionRepository;
        }

        public async void AddConnection(int userId, string signalrConnectionId)
        {
            var newConnection = new Connection() { UserId = userId, SignalRid = signalrConnectionId, TimeStamp = DateTime.Now };
            _connectionRepository.AddConnection(newConnection);
        }

        public async void RemoveConnection(string connectionId)
        {
            var tempConn = _connectionRepository.GetConnections().ToList().FirstOrDefault(conn => conn.SignalRid == connectionId);
            _connectionRepository.RemoveConnection(tempConn);
        }

        public async Task<Connection> GetConnection(int userId, string connectionId)
        {
            return _connectionRepository.GetConnections().FirstOrDefault(conn => conn.UserId == userId && conn.SignalRid == connectionId);
        }

        public List<Connection> GetConnections()
        {
            return _connectionRepository.GetConnections();
        }
        public List<Connection> GetConnections(IEnumerable<int> userIds)
        {
            return _connectionRepository.GetConnections().Where(conn => userIds.Contains(conn.UserId)).ToList();
        }
    }
}
