using Repository.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Repository.Interfaces
{
    public interface EventIRepository
    {
        Task<List<Event>> GetAllAsync();
        Task<Event> GetByIdAsync(int id);
        Task<Event> AddItemAsync(Event item);
        Task<Event> UpdateItemAsync(int id, Event item);
        Task DeleteItemAsync(int id);
        Task<List<Event>> GetEventsByProducerIdAsync(int producerId);
        Task<List<Event>> GetByDateAsync(DateTime date);
        Task<List<Event>> GetUpcomingEventsAsync();
        Task<List<Event>> SearchAsync(string searchTerm);
        Task<List<Event>> GetByLocationAsync(string location);
        Task<List<Event>> GetByHallIdAsync(int hallId);
    }
}