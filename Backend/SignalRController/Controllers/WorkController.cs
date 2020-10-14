using System;
using System.IO;
using System.Linq;
using System.Timers;
using Microsoft.AspNetCore.Hosting;
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
        private readonly Random _random;
        private static Timer _timer;
        //private static int timeOut = 5000;
        private static bool isWorking = false;
        

        public WorkController(IHubContext<UserHub> hubUser, IHubContext<AdminHub> hubAdmin, IHubContext<ImagesHub> hubImages, IWebHostEnvironment webHostEnvironment)
        {
            _hubUser = hubUser;
            _hubAdmin = hubAdmin;
            _hubImages = hubImages;
            _webHostEnvironment = webHostEnvironment;
            _random = new Random();
            _timer = new Timer(5000);
            _timer.AutoReset = false;
            _timer.Elapsed += SendMessagesFromTimer;
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
            _hubUser.Clients.All.SendAsync("GetMessages", new UserMessageModel { rand = _random.Next() });
        }

        public void SendMessage(MessageModel user)
        {
            
            if (!string.IsNullOrEmpty(user.Role))
            {
                _timer.Start();
                if (user.Role != "Admin")
                {
                    //_timer.Start();
                    SendMessageAdmin(user);
                }
            }
        }

        private void SendImages()
        {
            try
            {
                _hubImages.Clients.All.SendAsync(
                                        "GetImages", 
                                        new BlodModel
                                            (
                                                Convert.ToBase64String(System.IO.File.ReadAllBytes(Path.Combine(_webHostEnvironment.ContentRootPath, "Images", $"{ _random.Next(1, 10)}.png"))), 
                                                "data:image/png;base64,"
                                            )
                                        );
            }
            catch
            {

            }
        }

        private void SendMessageAdmin(MessageModel userName)
        {
            GetUsers(userName);
        }

        public void SetTimeOutFromAdmin(int sec)
        {
            //timeOut = sec;
            if (isWorking)
            {
                _timer.Stop();
                isWorking = !isWorking;
            }
            _timer.Interval = sec*1000;
            _timer.Start();
        }

        private void SendMessagesFromTimer(object sender, ElapsedEventArgs e)
        {
            if (!isWorking)
            {
                isWorking = !isWorking;
            }

            _timer.Enabled = false;
            SendImages();
            SendMessageUsers();
            _timer.Enabled = true;
        }
    }
}
