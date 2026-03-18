using Repository.Entities;
using Repository.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Storage;

namespace Repository.Repositories
{
    public class OrderDetailRepository : OrderDetailIRepository
    {
        private readonly IContext _context;
        public OrderDetailRepository(IContext context)
        {
            _context = context;
        }

        public async Task<OrderDetail> AddItemAsync(OrderDetail item)
        {
            await _context.OrderDetails.AddAsync(item);
            await _context.SaveChangesAsync();
            return item;
        }

        public async Task DeleteItemAsync(int id)
        {
            var orderDetail = await GetByIdAsync(id);
            if (orderDetail != null)
            {
                _context.OrderDetails.Remove(orderDetail);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<OrderDetail>> GetAllAsync()
        {
            return await _context.OrderDetails.ToListAsync();
        }

        public async Task<OrderDetail> GetByIdAsync(int id)
        {
            return await _context.OrderDetails.FirstOrDefaultAsync(x => x.Id == id);
        }
        public async Task<List<OrderDetail>> GetByUserIdAsync(int userId)
        {
            return await _context.OrderDetails
                .Where(x => x.UserID == userId)
                .ToListAsync();
        }
       
        public Task<IDbContextTransaction> BeginTransactionAsync()
        {
            var transaction = _context.Database.BeginTransaction();
            return Task.FromResult(transaction as IDbContextTransaction);
        }
        public async Task<List<OrderDetail>> GetCartAsync(int userId)
        {
            return await _context.OrderDetails
               .Where(o => o.UserID == userId && o.Status == OrderStatus.Reserved)
               .Include(o => o.HallSeat)
               .Include(o => o.Event)
               .ToListAsync();
        }
        public async Task<List<OrderDetail>> GetReservedSeatsByUser(int userId,int eventId)
        {
            return await _context.OrderDetails
               .Where(o => o.UserID == userId && o.EventID== eventId&& o.Status == OrderStatus.Reserved)
               .Include(o => o.HallSeat)
               .Include(o => o.Event)
               .ToListAsync();
        }
        public async Task<List<OrderDetail>> GetRealOrdersAsync(int userId)
        { 
            return await _context.OrderDetails
                .Where(o => o.UserID == userId && o.Status == OrderStatus.Sold)
                .Include(o => o.HallSeat)
                .Include(o => o.Event)
                .ToListAsync();
        }
        public async Task<List<int>> GetBookedSeatsByEvent(int eventId, List<int> seatIds)
        {
            return await _context.OrderDetails
                .Where(o => o.EventID == eventId && seatIds.Contains(o.HallSeatID))
                .Select(o => o.HallSeatID)
                .ToListAsync();
        }
        public async Task<List<OrderDetail>> GetBookedSeatsOrderByEvent(int eventId, List<int> seatIds)
        {
            return await _context.OrderDetails
                .Where(o => o.EventID == eventId && seatIds.Contains(o.HallSeatID))
                .Select(o => o)
                .ToListAsync();
        }



        public async Task<List<OrderDetail>> GetByEventIdAsync(int eventId)
        {
            return await _context.OrderDetails
                .Where(o => o.EventID == eventId && o.Status != OrderStatus.Cancelled)
                .ToListAsync();
        }
        public async Task<OrderDetail> UpdateItemAsync(int id, OrderDetail item)
        {
            var orderDetail = await GetByIdAsync(id);
            if (orderDetail != null)
            {
                orderDetail.EventID = item.EventID;
                orderDetail.UserID = item.UserID;
                orderDetail.PriceAtPurchase = item.PriceAtPurchase;
                orderDetail.Status = item.Status;
                orderDetail.HallSeatID = item.HallSeatID;
                orderDetail.SelectAt = item.SelectAt;

                await _context.SaveChangesAsync();
            }
            return orderDetail;
        }
        public async Task<List<OrderDetail>> UpdateItemsAsyncs(List<OrderDetail> items)
        {
            var updatedItems = new List<OrderDetail>();

            foreach (var item in items)
            {
                var orderDetail = await GetByIdAsync(item.Id);
                if (orderDetail != null)
                {
                    orderDetail.EventID = item.EventID;
                    orderDetail.UserID = item.UserID;
                    orderDetail.PriceAtPurchase = item.PriceAtPurchase;
                    orderDetail.Status = item.Status;
                    orderDetail.HallSeatID = item.HallSeatID;
                    orderDetail.SelectAt = item.SelectAt;

                    updatedItems.Add(orderDetail);
                }
            }

            // שמירה של כל השינויים במסד
            if (updatedItems.Any())
            {
                await _context.SaveChangesAsync();
            }

            return updatedItems;
        }
    }
}