using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Repository.Entities;
using Service.Dto;

namespace Service.Interface
{
    public interface HallSeatIService:IService<HallSeatDto>
    {
        Task<List<HallSeatDto>> GetByHallIdAsync(int hallId);
    }
}
