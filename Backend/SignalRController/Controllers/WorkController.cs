﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
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
        private readonly IHubContext<ImagesHub> _hubImages;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private static int timeOut = 5;
        private static bool isWorking = false;

        public WorkController(IHubContext<UserHub> hubUser, IHubContext<AdminHub> hubAdmin, IHubContext<ImagesHub> hubImages, IWebHostEnvironment webHostEnvironment)
        {
            _hubUser = hubUser;
            _hubAdmin = hubAdmin;
            _hubImages = hubImages;
            _webHostEnvironment = webHostEnvironment;
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

        private IActionResult GetUsers(MessageModel user)
        {
            _hubAdmin.Clients.All.SendAsync("GetUsers", new { Name=user.Name, Status=user.Status });
            return Ok();
        }

        public void SendMessageUsers()
        {
            if (isWorking)
                return;
            isWorking = true;
            while (true)
            {
                _hubUser.Clients.All.SendAsync("GetMessages", new UserMessageModel { rand = new Random().Next() });
                Thread.Sleep(timeOut * 1000);
            }
        }

        public void SendMessage(MessageModel user)
        {
            if (!string.IsNullOrEmpty(user.Role))
            {
                if(user.Role != "Admin")
                {
                    SendMessageAdmin(user);
                    SendMessageUsers();
                }
                SendImages();
            }
        }

        private void SendImages()
        {
            while (true)
            {
                var tt = _webHostEnvironment.ContentRootPath;
                var t = Path.Combine(tt,"Images", $"{ new Random().Next(1, 10).ToString()}.png");
                _hubImages.Clients.All.SendAsync("SendImages", new UserMessageModel { rand = new Random().Next() });
                Thread.Sleep(timeOut * 1000);
            }
        }

        private void SendMessageAdmin(MessageModel userName)
        {
            GetUsers(userName);
        }

        public void SetTimeOutFromAdmin(int sec)
        {
            timeOut = sec;
        }

    }
}