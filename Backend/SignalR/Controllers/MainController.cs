using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Timers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using SignalR.Hubs;
using SignalR.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SignalR.Controllers
{
    [Route("api/main")]
    [ApiController]
    public class MainController : ControllerBase
    {
        private readonly IHubContext<ClientsHub> _clientsHub;
        private readonly IHubContext<AdminsHub> _adminsHub;
        private readonly IHubContext<ImagesHub> _imagesHub;

        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly Random _random = new Random();

        private static Timer _timer = new Timer();
        private static string _nextImage;
        private static bool isWorking = false;
        private static ObservableCollection<UserActivitiesModel> _userActivities = new ObservableCollection<UserActivitiesModel>();
        private static ObservableCollection<ActiveUsersModel> _activeUsers = new ObservableCollection<ActiveUsersModel>();
        private static ObservableCollection<AdminsAccountsModel> _adminsAccountsColection = new ObservableCollection<AdminsAccountsModel>();

        public MainController(IHubContext<ClientsHub> clientsHub, IHubContext<AdminsHub> adminsHub, IHubContext<ImagesHub> imagesHub, IWebHostEnvironment webHostEnvironment)
        {
            _clientsHub = clientsHub;
            _adminsHub = adminsHub;
            _imagesHub = imagesHub;
            _webHostEnvironment = webHostEnvironment;
            

            if (_adminsAccountsColection.Count==0)
            {
                SetAdminsCollection();
                _timer.Interval = 5000;
                _timer.AutoReset = false;
                _timer.Elapsed += SendMessagesFromTimer;
                _timer.Start();
                _userActivities.CollectionChanged += _userActivities_CollectionChanged;
                _activeUsers.CollectionChanged += _activeUsers_CollectionChanged;
            }
        }

        [HttpGet]
        [Route("getRoles")]
        public JsonResult GetUserRole(string name)
        {
            if (_adminsAccountsColection.Any(a => a.Name == name))
            {
                return new JsonResult(new UsersRoleModel(name, "Admin"));
            }
            else
            {
                return new JsonResult(new UsersRoleModel(name, "User"));
            }
        }

        public static void UserConnect(string connectionId, string name, string role, int status)
        {
            //_userActivities.Add(new UserActivitiesModel(connectionId, name, role, status));
            _activeUsers.Add(new UserActivitiesModel(connectionId, name, role, status));
        }

        public static void UserDisconnect(string connectionId, string name, string role, int status)
        {
            var tempModel = new UserActivitiesModel(connectionId, name, role, status);
            //_userActivities.Add(tempModel);
            foreach(var user in _activeUsers)
            {
                if(user.ConnectionId == tempModel.ConnectionId)
                {
                    _activeUsers.Remove(user);
                    break;
                }
            }
        }

        internal static void UserDisconnect(string connectionId)
        {
            //var tempModel = new UserActivitiesModel(connectionId, name, role, status);
            //_userActivities.Add(tempModel);
            foreach (var user in _activeUsers)
            {
                if (user.ConnectionId == connectionId)
                {
                    _activeUsers.Remove(user);
                    break;
                }
            }
        }

        //[HttpGet]
        //[Route("getActiveUsers")]
        //public JsonResult GetActiveUsers()
        //{
        //    return new JsonResult(6);
        //}

        [HttpGet]
        [Route("getUsersActivities")]
        public JsonResult GetUsersActivities()
        {
            return new JsonResult(6);
        }

        [HttpPost]
        [Route("setTimeOut")]
        public void SetTimeOutFromAdmin([FromBody] TimeOutModel timeOutUserMesssage)
        {
            //timeOut = sec;
            if (isWorking)
            {
                _timer.Stop();
                isWorking = !isWorking;
            }
            _timer.Interval = timeOutUserMesssage.setTimeOut * 1000;
            _timer.Start();
        }

        private void _activeUsers_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            _adminsHub.Clients.All.SendAsync("SendActiveClients", _activeUsers);
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                SendMessage(e.NewItems[0], 1);
            }
            else 
                if (e.Action == NotifyCollectionChangedAction.Remove)
                {
                    SendMessage(e.OldItems[0], 0);
                }
        }

        private void SendMessage(object v, int status)
        {
            var t = (ActiveUsersModel)v;
            _adminsHub.Clients.All.SendAsync("SendActivitiesClient", new UserActivitiesModel(t.ConnectionId, t.Name, t.Role, status));
        }

        private void _userActivities_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            //if (e.Action == NotifyCollectionChangedAction.Add)
            //{
            //    _adminsHub.Clients.All.SendAsync("SendActivitiesClient", (UserActivitiesModel)e.NewItems[0]);
            //}
            //else
            //    if (e.Action == NotifyCollectionChangedAction.Remove)
            //    {
            //        _adminsHub.Clients.All.SendAsync("SendActivitiesClient", (UserActivitiesModel)e.OldItems[0]);
            //    }
        }

        private void SendMessagesFromTimer(object sender, ElapsedEventArgs e)
        {

            if (!isWorking)
            {
                isWorking = !isWorking;
            }

            _timer.Enabled = false;
            SendImages();
            SendMessageForClients();
            _timer.Enabled = true;
        }

        private void SendMessageForClients()
        {
            _clientsHub.Clients.All.SendAsync("GetMessages", new { rand = _random.Next() });
        }

        private void SendImages()
        {
            if (!string.IsNullOrEmpty(_nextImage))
            {
                _imagesHub.Clients.All.SendAsync("GetImages", new ImageModel(_nextImage));
            }
            _nextImage = GetNextImage();
            _adminsHub.Clients.All.SendAsync("GetNextImages", new ImageModel(_nextImage));
        }

        private string GetNextImage()
        {
            return Convert.ToBase64String(System.IO.File.ReadAllBytes(Path.Combine(_webHostEnvironment.ContentRootPath, "Images", $"{ _random.Next(1, 10)}.png")));
        }

        private void SetAdminsCollection()
        {
            _adminsAccountsColection.Add(new AdminsAccountsModel("Admin"));
            _adminsAccountsColection.Add(new AdminsAccountsModel("Admin1"));
            _adminsAccountsColection.Add(new AdminsAccountsModel("Admin2"));
            _adminsAccountsColection.Add(new AdminsAccountsModel("Admin3"));
        }

    }
}
