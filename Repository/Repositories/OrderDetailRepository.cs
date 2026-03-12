using Repository.Entities;
using Repository.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

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
    }
}