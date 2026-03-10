using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Repository.Entities;
using Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

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
                await ExpireReservedTicketsAsync();
                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
        }

        private async Task ExpireReservedTicketsAsync()
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<IContext>();

                var now = DateTime.Now;

                // בוחרים את כל המושבים שהסטטוס שלהם Reserved והזמן עבר
                var expiredTickets = await context.OrderDetails
                    .Where(o => o.Status == OrderStatus.Reserved
                             && o.SelectAt.AddMinutes(10) < now)
                    .ToListAsync();

                if (expiredTickets.Any())
                {
                    context.OrderDetails.RemoveRange(expiredTickets);
                    await context.SaveChangesAsync();
                }
            }
        }
    }
}