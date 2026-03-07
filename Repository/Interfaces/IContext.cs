using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Repository.Entities;
using Microsoft.EntityFrameworkCore;

namespace Repository.Interfaces
{
    public interface IContext
    {
        public DbSet<User> Users { get; }
        public DbSet<Event> Events { get; }
        public DbSet<Producer> Producers { get; }
        public DbSet<HallSeat> HallSeats { get; }
        public DbSet<Hall> Halls { get; }
        public DbSet<OrderDetail> OrderDetails { get; }
        public void save();
    }
}
