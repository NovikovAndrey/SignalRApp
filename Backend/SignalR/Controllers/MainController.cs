using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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

        private static Timer _timer;
        private static string _nextImage;
        private static bool isWorking = false;
        private static ObservableCollection<UserActivitiesModel> _userActivitiesModel = new ObservableCollection<UserActivitiesModel>();
        private static ObservableCollection<ActiveUsersModel> _activeUsersModel = new ObservableCollection<ActiveUsersModel>();

        public MainController(IHubContext<ClientsHub> clientsHub, IHubContext<AdminsHub> adminsHub, IHubContext<ImagesHub> imagesHub, IWebHostEnvironment webHostEnvironment)
        {
            _clientsHub = clientsHub;
            _adminsHub = adminsHub;
            _imagesHub = imagesHub;
            _webHostEnvironment = webHostEnvironment;
        }
        // GET: api/<MainController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<MainController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<MainController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<MainController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<MainController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
