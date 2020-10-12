using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.AspNetCore.SignalR;
using SignalRServer.Hubs;
using SignalRServer.Models;
using SignalRServer.Services;

namespace SignalRServer.Controllers
{
    [Route("api/chat")]
    [ApiController]
    public class MessageController : ControllerBase
    {
        private IHubContext<SendHub> _hubContext;
        private string _adminGroup = "Admin", _userGroup = "Users";
             
        public MessageController(IHubContext<SendHub> hubContext)
        {
            _hubContext = hubContext;
        }

        //[HttpGet]
        public void SendMessage()
        {
            while (true)
            {
                _hubContext.Clients.All.SendAsync("GetMessage1", new MessageModel { rand = new Random().Next() });
                Thread.Sleep(1000);
            }
        }

        [HttpPost]
        [Route("sendUser")]
        public Task SendUserMessage([FromBody]UserModel userName)
        {

            if(userName.name=="Admin")
            {
               return _hubContext.Clients.All.SendAsync("GetUsers", "dsfsdfds");
            }
            else
            {
                return _hubContext.Clients.All.SendAsync("GetMessage", 2312312432);
            }
            //_hubContext.Clients.All.SendAsync("SendUserMessage", userName);
            //return Ok();
        }
    }
}
