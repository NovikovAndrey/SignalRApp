using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using SignalRServer.Hubs;
using SignalRServer.Interfaces;
using SignalRServer.Models;
using System;
using System.Timers;

namespace SignalRServer.Services
{
    public class SendMessageService: Hub<ISendMessage>
    {
        private IHubContext<SendHub> _hubContext;
        private Timer _timer;
        public SendMessageService(IHubContext<SendHub> hubContext)
        {
            _hubContext = hubContext;
            StartSend();

        }

        public void StartSend()
        {
            _timer = new Timer(5000)
            {
                AutoReset = false
            };
            _timer.Elapsed += _timer_Elapsed;
            _timer.Start();
        }

        private void _timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            _timer.Enabled = false;

            _hubContext.Clients.All.SendAsync("GetMessage1", new MessageModel {rand= new Random().Next() });
            //_hubContext.Clients.All.SendAsync("GetTest", "dsfsdfds");
            _timer.Enabled = true;
        }
    }
}
