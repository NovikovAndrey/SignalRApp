using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using SignalRController.Hubs;
using SignalRController.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SignalRController.Controllers
{
    [Produces("application/json")]
    [Route("api/admin")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly IHubContext<AdminHub> _hubContext;

        public AdminController(IHubContext<AdminHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public IActionResult GetUsers(AdminMessagesModel userName)
        {
            _hubContext.Clients.All.SendAsync("GetUsers", new AdminMessagesModel(userName.Name, userName.Status));
            return Ok();
        }
    }
}
