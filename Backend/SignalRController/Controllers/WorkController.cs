using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
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
        private int timeOut = 5;
        private bool Work = true;
        private static bool isWorking = false;


        public WorkController(IHubContext<UserHub> hubUser, IHubContext<AdminHub> hubAdmin)
        {
            _hubUser = hubUser;
            _hubAdmin = hubAdmin;
            //_userController = userController;
            //_adminController = adminController;
        }

        [HttpPost]
        [Route("getUserRole")]
        public JsonResult GetUserRole([FromBody] AdminMessagesModel user)
        {
            if ((new[] { "Admin", "Admin1", "Admin2" }).Contains(user.Name))
            {
                return new JsonResult(new UserRoleModel(user.Name, "Admin"));
            }
            else
            {
                return new JsonResult(new UserRoleModel(user.Name, "User"));
            }

        }

        private IActionResult GetUsers(AdminMessagesModel userName)
        {
            _hubAdmin.Clients.All.SendAsync("GetUsers", new AdminMessagesModel(userName.Name, userName.Status));
            return Ok();
        }

        public void SendMessageUsers()
        {
            if (isWorking)
                return;

            isWorking = true;

            var t = timeOut;
            while (true)
            {
                _hubUser.Clients.All.SendAsync("GetMessages", new UserMessageModel { rand = new Random().Next() });
                Thread.Sleep(timeOut * 1000);
            }
        }

        public void SendMessage(AdminMessagesModel userName)
        {
            if (!string.IsNullOrEmpty(userName.Name))
            {
                //if (!userName.Name.Equals("Admin"))
                if(!(new[] {"Admin", "Admin1", "Admin2"}).Contains(userName.Name))
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
            //Work = false;
            timeOut = sec;
            //Work = true;

        }

    }
}
