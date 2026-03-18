using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Repository.Entities;
using Microsoft.EntityFrameworkCore.Storage;
namespace Repository.Interfaces
{
    public interface OrderDetailIRepository:IRepository<OrderDetail>
    {
        Task<List<OrderDetail>> GetByUserIdAsync(int UserId);
        Task<List<OrderDetail>> GetByEventIdAsync(int eventId);
        Task<IDbContextTransaction> BeginTransactionAsync();
        Task<List<int>> GetBookedSeatsByEvent(int eventId, List<int> seatIds);
        Task<List<OrderDetail>> GetCartAsync(int userId);
        Task<List<OrderDetail>> GetRealOrdersAsync(int userId);
        Task<List<OrderDetail>> GetReservedSeatsByUser(int userId, int eventId);
        Task<List<OrderDetail>> UpdateItemsAsyncs(List<OrderDetail> items);






    }
}
