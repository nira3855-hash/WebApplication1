using Microsoft.EntityFrameworkCore.Storage;
using Repository.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Repository.Interfaces
{
    public interface OrderDetailIRepository : IRepository<OrderDetail>
    {
        Task<List<OrderDetail>> GetByUserIdAsync(int userId);
        Task<List<OrderDetail>> GetByEventIdAsync(int eventId);
        Task<IDbContextTransaction> BeginTransactionAsync();
        Task<List<int>> GetBookedSeatsByEvent(int eventId, List<int> seatIds);
        Task<List<OrderDetail>> GetCartAsync(int userId);
        Task<List<OrderDetail>> GetRealOrdersAsync(int userId);
        Task<List<OrderDetail>> GetReservedSeatsByUser(int userId, int eventId);
        Task<List<OrderDetail>> UpdateItemsAsyncs(List<OrderDetail> items);
    }
}