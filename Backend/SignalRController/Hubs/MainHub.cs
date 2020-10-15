using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SignalR;
using SignalRController.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;

namespace SignalRController.Hubs
{
    public class MainHub: Hub
    {
        private readonly IHubContext<UserHub> _hubUser;
        private readonly IHubContext<AdminHub> _hubAdmin;
        private readonly IHubContext<ImagesHub> _hubImages;
        private readonly IWebHostEnvironment _webHostEnvironment;

        private readonly Random _random;
        private static Timer _timer;
        private static string _nextImage;
        private static bool isWorking = false;

        public MainHub(IHubContext<UserHub> hubUser, IHubContext<AdminHub> hubAdmin, IHubContext<ImagesHub> hubImages, IWebHostEnvironment webHostEnvironment)
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
            return Convert.ToBase64String(File.ReadAllBytes(Path.Combine(_webHostEnvironment.ContentRootPath, "Images", $"{ _random.Next(1, 10)}.png")));
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
                    SendMessageAdmin(user);
                }
            }
        }

        private void SendMessageAdmin(MessageModel userName)
        {
            GetUsers(userName);
        }

        private void GetUsers(MessageModel user)
        {
            _hubAdmin.Clients.All.SendAsync("GetUsers", new { Name = user.Name, Status = user.Status });
        }
    }
}
