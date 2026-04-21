using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using RETAIL.BASE.NEG.Services;

namespace RETAIL.BASE.API.Controllers.Hubs
{
     public class DataHub : Hub
    {

        private readonly HubCommunicationService _hubCommunicationService;

        public DataHub(HubCommunicationService hubCommunicationService)
        {
            _hubCommunicationService = hubCommunicationService;
        }

        public override async Task OnConnectedAsync()
        {
            string userName = Context.GetHttpContext()?.Request?.Query["userName"];
            await _hubCommunicationService.AddToGroupAndRegisterAsync(userId: Context.ConnectionId, userName: userName);
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            _hubCommunicationService.RemoveConnection(Context.ConnectionId);
            await base.OnDisconnectedAsync(exception);
        }

        public async Task SendUpdate()
        {
            await Clients.All.SendAsync("ReceiveUpdate");
        }

        public async Task SendMessage(string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", message);
        }

        public async Task SendMessageToUser(string userName, string message)
        {
            _hubCommunicationService.GetHookConnectionsForUser(userName).ToList().ForEach(async userId =>
            {
                await Clients.Client(userId).SendAsync("ReceiveMessage", message);
            });
        }

    }
}
