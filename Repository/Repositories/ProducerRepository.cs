using Microsoft.EntityFrameworkCore;
using Repository.Entities;
using Repository.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Repository.Repositories
{
    public class ProducerRepository : IRepository<Producer>
    {
        private readonly IContext _context;
        public ProducerRepository(IContext context)
        {
            _context = context;
        }

        public async Task<Producer> AddItemAsync(Producer item)
        {
            await _context.Producers.AddAsync(item);
            await _context.SaveChangesAsync();
            return item;
        }

        public async Task DeleteItemAsync(int id)
        {
            var producer = await GetByIdAsync(id);
            if (producer != null)
            {
                _context.Producers.Remove(producer);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<Producer>> GetAllAsync()
        {
            return await _context.Producers.Include(p => p.User).ToListAsync();
        }

        public async Task<Producer> GetByIdAsync(int id)
        {
            return await _context.Producers
                .Include(p => p.User)
                .FirstOrDefaultAsync(x => x.UserId == id);
        }

        public async Task<Producer> UpdateItemAsync(int id, Producer item)
        {
            var producer = await GetByIdAsync(id);
            if (producer != null)
            {
                producer.CompanyName = item.CompanyName;
                producer.UserId = item.UserId;
                producer.User = item.User;
                producer.Bio = item.Bio;

                await _context.SaveChangesAsync();
            }
            return producer;
        }
    }
}