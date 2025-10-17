using Microsoft.AspNetCore.SignalR;

namespace Ecommerce.API.Hubs
{
    public class NotificationHub:Hub
    {
        public override Task OnConnectedAsync()
        {
            Console.WriteLine($"User connected: {Context.UserIdentifier}");
            return base.OnConnectedAsync();
        }
    }
}
