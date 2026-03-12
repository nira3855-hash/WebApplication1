using Repository.Entities;
using Repository.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Repository.Repositories
{
    public class HallSeatRepository : HallSeatIRepository
    {
        private readonly IContext _context;
        public HallSeatRepository(IContext context)
        {
            _context = context;
        }
        public async Task AddRangeAsync(List<HallSeat> seats)
        {
            if (seats == null || !seats.Any())
                return;

            await _context.HallSeats.AddRangeAsync(seats);
            await _context.SaveChangesAsync();
        }
        public async Task<HallSeat> AddItemAsync(HallSeat item)
        {
            await _context.HallSeats.AddAsync(item);
            await _context.SaveChangesAsync();
            return item;
        }

        public async Task DeleteItemAsync(int id)
        {
            var hallSeat = await GetByIdAsync(id);
            if (hallSeat != null)
            {
                _context.HallSeats.Remove(hallSeat);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<HallSeat>> GetAllAsync()
        {
            return await _context.HallSeats.ToListAsync();
        }

        public async Task<HallSeat> GetByIdAsync(int id)
        {
            return await _context.HallSeats.FirstOrDefaultAsync(x => x.Id == id);
        }
        public async Task<List<HallSeat>> GetByHallIdAsync(int hallId)
        {
            return await _context.HallSeats
                .Where(x => x.HallID == hallId)
                .ToListAsync();
        }
        public async Task<HallSeat> UpdateItemAsync(int id, HallSeat item)
        {
            var hallSeat = await GetByIdAsync(id);
            if (hallSeat != null)
            {
                hallSeat.HallID = item.HallID;
                hallSeat.RowNumber = item.RowNumber;
                hallSeat.SeatNumber = item.SeatNumber;
                hallSeat.AddPrice = item.AddPrice;
                hallSeat.TypeOfPlace = item.TypeOfPlace;

                await _context.SaveChangesAsync();
            }
            return hallSeat;
        }
    }
}