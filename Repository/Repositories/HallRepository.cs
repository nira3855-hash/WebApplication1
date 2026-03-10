using Repository.Entities;
using Repository.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Repository.Repositories
{
    public class HallRepository : IRepository<Hall>
    {
        private readonly IContext _context;
        public HallRepository(IContext context)
        {
            _context = context;
        }

        public async Task<Hall> AddItemAsync(Hall item)
        {
            await _context.Halls.AddAsync(item);
            await _context.SaveChangesAsync();
            return item;
        }

        public async Task DeleteItemAsync(int id)
        {
            var hall = await GetByIdAsync(id);
            if (hall != null)
            {
                _context.Halls.Remove(hall);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<Hall>> GetAllAsync()
        {
            return await _context.Halls.ToListAsync();
        }

        public async Task<Hall> GetByIdAsync(int id)
        {
            return await _context.Halls.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<Hall> UpdateItemAsync(int id, Hall item)
        {
            var hall = await GetByIdAsync(id);
            if (hall != null)
            {
                hall.shape = item.shape;
                hall.name = item.name;
                hall.numOfSeats = item.numOfSeats;
                hall.location = item.location;

                await _context.SaveChangesAsync();
            }
            return hall;
        }
    }
}