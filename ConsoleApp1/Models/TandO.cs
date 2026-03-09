using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Repository.Entities;
using Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1.Models
{
    public class TandO : DbContext, IContext
    {
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Event> Events { get; set; }
        public virtual DbSet<Producer> Producers { get; set; }
        public virtual DbSet<HallSeat> HallSeats { get; set; }
        public virtual DbSet<Hall> Halls { get; set; }
        public virtual DbSet<OrderDetail> OrderDetails { get; set; }

        DbSet<User> IContext.Users => Users;
        DbSet<Event> IContext.Events => Events;
        DbSet<Producer> IContext.Producers => Producers;
        DbSet<HallSeat> IContext.HallSeats => HallSeats;
        DbSet<Hall> IContext.Halls => Halls;
        DbSet<OrderDetail> IContext.OrderDetails => OrderDetails;


        public void save()
        {
            SaveChanges();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //סמינר 
            //optionsBuilder.UseSqlServer("server=sql;database=TandO;trusted_connection=true;TrustServerCertificate=True");
            // תמר
             //optionsBuilder.UseSqlServer("server=TAMARCOMPUTER\\MSSQLSERVER01;database=TandO;trusted_connection=true;TrustServerCertificate=True");
            //אור חן
            optionsBuilder.UseSqlServer("server=DESKTOP-9R4N6KE\\SQLEXPRESS01;database=TandO;trusted_connection=true;TrustServerCertificate=True");
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            

            // מגדיר שמו של הטבלה במפורש
            modelBuilder.Entity<User>().ToTable("User");
            modelBuilder.Entity<Event>().ToTable("Event");
            modelBuilder.Entity<Producer>().ToTable("Producer");
            modelBuilder.Entity<HallSeat>().ToTable("HallSeat");
            modelBuilder.Entity<Hall>().ToTable("Hall");
            modelBuilder.Entity<OrderDetail>().ToTable("OrderDetail");
            modelBuilder.Entity<OrderDetail>()
    .HasIndex(o => new { o.EventID, o.HallSeatID })
    .IsUnique();

        }
    }
}

