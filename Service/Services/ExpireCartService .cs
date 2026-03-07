using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading;
using System.Threading.Tasks;
using Repository.Entities;
using Repository;
using Microsoft.EntityFrameworkCore;
using Repository.Interfaces;

namespace Service.Services
{


    public class ExpireCartService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;

        public ExpireCartService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await ExpireReservedTickets();
                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken); // כל דקה
            }
        }

        private async Task ExpireReservedTickets()
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<IContext>();

                var now = DateTime.Now;

                // בוחרים את כל המושבים שהסטטוס שלהם Reserved והזמן עבר
                var expiredTickets = context.OrderDetails
                    .Where(o => o.Status == OrderStatus.Reserved
                             && o.SelectAt.AddMinutes(10) < now)
                    .ToList();

                if (expiredTickets.Any())
                {
                    // מוחקים את הרשומות
                    context.OrderDetails.RemoveRange(expiredTickets);
                    context.save();
                }
            }
        }
    }
}