using Repository.Entities;
using Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore; // צריך בשביל ToListAsync, FirstOrDefaultAsync

namespace Repository.Repositories
{
    public class EventRepository : EventIRepository
    {
        private readonly IContext _context;
        public EventRepository(IContext context)
        {
            this._context = context;
        }

        public async Task<Event> AddItemAsync(Event item)
        {
            await _context.Events.AddAsync(item);
            await _context.SaveChangesAsync();
            return item;
        }

        public async Task DeleteItemAsync(int id)
        {
            var entity = await GetByIdAsync(id);
            if (entity != null)
            {
                _context.Events.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<Event>> GetAllAsync()
        {
            return await _context.Events.ToListAsync();
        }

        public async Task<Event> GetByIdAsync(int id)
        {
            return await _context.Events.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<Event> UpdateItemAsync(int id, Event item)
        {
            var existingEvent = await GetByIdAsync(id);
            if (existingEvent != null)
            {
                existingEvent.Title = item.Title;
                existingEvent.EventDate = item.EventDate;
                existingEvent.HallID = item.HallID;
                existingEvent.BasePrice = item.BasePrice;
                existingEvent.Location = item.Location; // ודאי שגם מיקום מעודכן
                existingEvent.Describe = item.Describe;

                // עדכון תמונה רק אם נשלחה תמונה חדשה
                if (!string.IsNullOrEmpty(item.ImageUrl))
                {
                    existingEvent.ImageUrl = item.ImageUrl;
                }

                await _context.SaveChangesAsync();
            }
            return existingEvent;
        }

        public async Task<List<Event>> GetEventsByProducerIdAsync(int producerId)
        {
            return await _context.Events.Where(e => e.ProducerID == producerId).ToListAsync();
        }

        public async Task<List<Event>> GetByDateAsync(DateTime date)
        {
            return await _context.Events
                .Where(e => e.EventDate.Date == date.Date)
                .ToListAsync();
        }

        public async Task<List<Event>> GetUpcomingEventsAsync()
        {
            return await _context.Events
                .Where(e => e.EventDate > DateTime.Now)
                .ToListAsync();
        }

        public async Task<List<Event>> SearchAsync(string searchTerm)
        {
            return await _context.Events
                .Where(e => e.Title.Contains(searchTerm) || e.Describe.Contains(searchTerm))
                .ToListAsync();
        }

        public async Task<List<Event>> GetByLocationAsync(string location)
        {
            return await _context.Events
                .Where(e => e.Location.Equals(location, StringComparison.OrdinalIgnoreCase))
                .ToListAsync();
        }

        public async Task<List<Event>> GetByHallIdAsync(int hallId)
        {
            return await _context.Events
                .Where(e => e.HallID == hallId)
                .ToListAsync();   
        }
    }
}