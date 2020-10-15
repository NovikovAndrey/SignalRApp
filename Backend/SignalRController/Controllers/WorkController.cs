using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Timers;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Connections.Features;
using Microsoft.AspNetCore.Identity;
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
        private static string _nextImage;
        private static bool isWorking = false;
        private static ObservableCollection<AdminMessagesModel> _adminMessagesCollection = new ObservableCollection<AdminMessagesModel>();

        
        public WorkController(IHubContext<UserHub> hubUser, IHubContext<AdminHub> hubAdmin, IHubContext<ImagesHub> hubImages, IWebHostEnvironment webHostEnvironment)
        {
            _hubUser = hubUser;
            _hubAdmin = hubAdmin;
            _hubImages = hubImages;
            _webHostEnvironment = webHostEnvironment;
            _random = new Random();
            _timer = new Timer(5000)
            {
                AutoReset = false
            };
            _timer.Elapsed += SendMessagesFromTimer;
            _adminMessagesCollection.CollectionChanged += GetUsersFromHistory;

        }

        private void GetUsersFromHistory(object sender, NotifyCollectionChangedEventArgs e)
        {
            var t = e.NewItems;
        }

        [HttpPost]
        [Route("getUserRole")]
        public JsonResult GetUserRole([FromBody] AdminMessagesModel user)
        {
            if ((new[] { "Admin", "Admin1", "Admin2" }).Contains(user.Name))
            {
                if (_adminMessagesCollection.Count>0)
                {
                    foreach(var t in _adminMessagesCollection)
                    {
                        GetUsers(t);
                    }
                }
                return new JsonResult(new UserRoleModel(user.Name, "Admin"));
            }
            else
            {
                _adminMessagesCollection.Add(user);
                return new JsonResult(new UserRoleModel(user.Name, "User"));
            }
        }

        private void GetUsers(AdminMessagesModel user)
        {
            _hubAdmin.Clients.All.SendAsync("GetUsers", user);
            //return Ok();
        }

        public void SendMessageUsers()
        {
            _hubUser.Clients.All.SendAsync("GetMessages", new UserMessageModel { rand = _random.Next() });
        }

        public void SendMessage(MessageModel user)
        {
            if (!string.IsNullOrEmpty(user.Role))
            {
                if (!isWorking)
                {
                    _timer.Start();
                }
                if (user.Role != "Admin")
                {
                    //_timer.Start();
                    SendMessageAdmin(new AdminMessagesModel(user.Name, user.Status));
                }
            }
        }

        private void SendImages()
        {
            if (!string.IsNullOrEmpty(_nextImage))
            {
                _hubImages.Clients.All.SendAsync(
                                        "GetImages",
                                        new BlodModel
                                            (
                                                _nextImage,
                                                "data:image/png;base64,"
                                            )
                                        );
            }
            _nextImage = GetNextImage();
            _hubAdmin.Clients.All.SendAsync(
                                    "GetNextImages",
                                    new BlodModel
                                        (
                                            _nextImage,
                                            "data:image/png;base64,"
                                        )
                                    );
        }

        private string GetNextImage()
        {
            return Convert.ToBase64String(System.IO.File.ReadAllBytes(Path.Combine(_webHostEnvironment.ContentRootPath, "Images", $"{ _random.Next(1, 10)}.png")));
        }

        private void SendMessageAdmin(AdminMessagesModel userName)
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
