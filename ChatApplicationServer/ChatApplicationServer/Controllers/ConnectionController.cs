using ChatApplicationServer.Models;
using ChatApplicationServer.Repository;
using Microsoft.AspNetCore.Mvc;

namespace ChatApplicationServer.Controllers
{
    [Controller]
    public class ConnectionController : Controller
    {
        private IConnectionsRepository _connectionsRepository;

        public ConnectionController(IConnectionsRepository connectionsRepository)
        {
            _connectionsRepository = connectionsRepository;
        }

        public List<Connection> GetCurrentConnections()
        {
            return _connectionsRepository.GetConnections();
        }

    }
}
