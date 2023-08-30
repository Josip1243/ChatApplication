using ChatApplicationServer.Models;
using ChatApplicationServer.Repository;
using Microsoft.AspNetCore.Mvc;

namespace ChatApplicationServer.Controllers
{
    [Controller]
    public class ConnectionController : Controller
    {
        private ConnectionsRepositoryMock _connectionsRepository;

        public ConnectionController(ConnectionsRepositoryMock connectionsRepository)
        {
            _connectionsRepository = connectionsRepository;
        }

        public List<Connection> GetCurrentConnections()
        {
            return _connectionsRepository.GetConnections();
        }

    }
}
