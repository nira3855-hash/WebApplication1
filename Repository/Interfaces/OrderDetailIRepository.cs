using Microsoft.EntityFrameworkCore.Storage;
using Repository.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Interfaces
{
    public interface OrderDetailIRepository:IRepository<OrderDetail>
    {
        Task<List<OrderDetail>> GetByUserIdAsync(int UserId);
        Task<List<OrderDetail>> GetByEventIdAsync(int eventId);
        Task<IDbContextTransaction> BeginTransactionAsync();
        Task<List<int>> GetBookedSeatsByEvent(int eventId, List<int> seatIds);
    }
}
