using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Repository.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Repository.Interfaces
{
    public interface IContext
    {
        DbSet<User> Users { get; }
        DbSet<Event> Events { get; }
        DbSet<Producer> Producers { get; }
        DbSet<HallSeat> HallSeats { get; }
        DbSet<Hall> Halls { get; }
        DbSet<OrderDetail> OrderDetails { get; }

        Task SaveChangesAsync();
        DatabaseFacade Database { get; }
    }
}