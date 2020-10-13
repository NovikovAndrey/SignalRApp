using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using SignalRController.Hubs;
using SignalRController.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SignalRController.Controllers
{
    [Produces("application/json")]
    [Route("api/admin")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly WorkController _workController;

        public AdminController(WorkController workController)
        {
            _workController = workController;
        }

        [HttpPost]
        [Route("setTimeOut")]
        public void SetTimeOut([FromBody] TimeOutModel timeOutUserMesssage)
        {
            _workController.SetTimeOutFromAdmin(timeOutUserMesssage.setTimeOut);
        }

    }
}
