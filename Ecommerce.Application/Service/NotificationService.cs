using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ecommerce.API.Hubs;
using Ecommerce.Domain.Entity;
using Ecommerce.Domain.Repository.Interfaces;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Application.Service
{
    public class NotificationService(IUnitOfWork unit, IHubContext<NotificationHub> hubContext)
    {
        private readonly IUnitOfWork _unit = unit;
        private readonly IHubContext<NotificationHub> _hubContext = hubContext;

        public async Task SendNotificationAsync(string userId, string message)
        {
            var notification = new Notification
            {
                UserId = userId,
                Message = message,
                CreatedAt = DateTime.UtcNow
            };

            await _unit.Notifications.AddAsync(notification);
            await _unit.SaveChangesAsync();

            await _hubContext.Clients.User(userId).SendAsync("ReceiveNotification", notification);
        }

        public async Task<List<Notification>> GetUserNotificationsAsync(string userId)
        {
            return await _unit.Notifications.Query()
                .Where(n => n.UserId == userId)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();
        }
    }
}
