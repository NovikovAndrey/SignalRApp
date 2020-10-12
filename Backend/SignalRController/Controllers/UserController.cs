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
        private readonly IHubContext<UserHub> _hubContext;
        private readonly AdminController _adminController;

        public UserController(IHubContext<UserHub> hubContext, AdminController adminController)
        {
            _hubContext = hubContext;
            _adminController = adminController;
        }
        // GET: api/<UserController>
        [HttpGet]
        public ActionResult Get()
        {
            _hubContext.Clients.All.SendAsync("GetMessage", "test", new Random().Next());
            return Ok();
        }

        // GET api/<UserController>/5
        [HttpPost]
        [Route("sendUser")]
        public IActionResult SendAsync([FromBody] AdminMessagesModel userName)
        {
            if (!string.IsNullOrEmpty(userName.Name))
            {
                if (userName.Name.Equals("Admin"))
                {

                }
                else
                {
                    SendMessageAdmin(userName);
                    SendMessageUsers();
                }
                return Ok();
            }
            else
            {
                return BadRequest();
            }
        }

        private IActionResult SendMessageUsers()
        {
            while (true)
            {
                _hubContext.Clients.All.SendAsync("GetMessages", new UserMessageModel { rand = new Random().Next() });
                Thread.Sleep(5000);
            }
        }

        private void SendMessageAdmin(AdminMessagesModel userName)
        {
            _adminController.GetUsers(userName);
        }
    }
}
