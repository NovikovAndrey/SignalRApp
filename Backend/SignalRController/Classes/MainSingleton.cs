using SignalRController.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SignalRController.Classes
{
    public class MainSingleton
    {
        private readonly WorkController _workController;

        public MainSingleton(WorkController workController)
        {
            _workController = workController;
        }
    }
}
