using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Repository.Entities;
namespace Repository.Interfaces
{
    public interface HallSeatIRepository:IRepository<HallSeat>
    {
        Task AddRangeAsync(List<HallSeat> seats);
        Task<List<HallSeat>> GetByHallIdAsync(int hallId);
    }
}
