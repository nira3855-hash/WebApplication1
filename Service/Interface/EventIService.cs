using Repository.Entities;
using Service.Dto;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service.Interface
{
    public interface EventIService
    {
        // מוסיף אירוע חדש
        Task<EventDto> AddEventAsync(EventDto item);

        // מוחק אירוע לפי ID
        Task DeleteEventAsync(int id);

        // מחזיר את כל האירועים
        Task<List<EventDto>> GetAllEventsAsync();

        // מחזיר אירוע לפי ID
        Task<EventDto> GetEventByIdAsync(int id);

        // מעדכן אירוע קיים לפי ID
        Task UpdateEventAsync(int id, EventDto item);

        // מחזיר את כל האירועים של מפיק מסוים
        Task<List<Event>> GetEventsByProducerIdAsync(int producerId);

        // מחזיר את כל האירועים בתאריך מסוים
        Task<List<Event>> GetEventsByDateAsync(DateTime date);

        // מחזיר את כל האירועים הקרבים
        Task<List<Event>> GetUpcomingEventsAsync();

        // מחפש אירועים לפי מחרוזת חיפוש
        Task<List<Event>> SearchEventsAsync(string searchTerm);

        // מחזיר אירועים לפי מיקום
        Task<List<Event>> GetEventsByLocationAsync(string location);

        // מחזיר אירועים לפי אולם
        Task<List<Event>> GetEventsByHallIdAsync(int hallId);

        // private ValidateEvent לא צריך להיות כאן
    }
}