using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using SignalRController.Hubs;
using SignalRController.Models;

namespace SignalRController.Controllers
{
    [Route("api/work")]
    [ApiController]
    public class WorkController : ControllerBase
    {
        private readonly IHubContext<UserHub> _hubUser;
        private readonly IHubContext<AdminHub> _hubAdmin;
        private readonly UserController _userController;
        private readonly AdminController _adminController;
        private int timeOut = 20;
        private bool Work = true;


        public WorkController(IHubContext<UserHub> hubUser, IHubContext<AdminHub> hubAdmin)
        {
            _hubUser = hubUser;
            _hubAdmin = hubAdmin;
            //_userController = userController;
            //_adminController = adminController;
        }

        private IActionResult GetUsers(AdminMessagesModel userName)
        {
            _hubAdmin.Clients.All.SendAsync("GetUsers", new AdminMessagesModel(userName.Name, userName.Status));
            return Ok();
        }

        public void SendMessageUsers()
        {
            var t = timeOut;
            while (Work)
            {
                _hubUser.Clients.All.SendAsync("GetMessages", new UserMessageModel { rand = new Random().Next() });
                Thread.Sleep(timeOut * 1000);
            }
        }

        internal void SendMessage(AdminMessagesModel userName)
        {
            if (!string.IsNullOrEmpty(userName.Name))
            {
                if (!userName.Name.Equals("Admin"))
                {
                    SendMessageAdmin(userName);
                    SendMessageUsers();
                }
            }
        }

        private void SendMessageAdmin(AdminMessagesModel userName)
        {
            GetUsers(userName);
        }

        public void SetTimeOutFromAdmin(int sec)
        {
            Work = false;
            timeOut = sec;
            Work = true;

        }

    }
}
