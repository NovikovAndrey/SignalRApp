using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using SignalRController.Hubs;
using SignalRController.Models;
using Microsoft.AspNetCore.Http;
using System.Threading;
using Microsoft.Extensions.DependencyInjection;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SignalRController.Controllers
{
    [Produces("application/json")]
    [Route("api/users")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly WorkController _workController;

        public UserController(WorkController workController)
        {
            _workController = workController;
        }

        [HttpPost]
        [Route("sendUser")]
        public void SendAsync([FromBody] AdminMessagesModel userName)
        {
            _workController.SendMessage(userName);
        }
    }
}
