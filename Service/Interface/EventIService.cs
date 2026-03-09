using Repository.Entities;
using Service.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Interface
{
    public interface EventIService
    {
        EventDto AddEvent(EventDto item);
        void DeleteEvent(int id);
        List<EventDto> GetAllEvents();
        EventDto GetEventById(int id);
        void UpdateEvent(int id, EventDto item);
        List<Event> GetEventsByProducerId(int producerId);
        List<Event> GetEventsByDate(DateTime date);
        List<Event> GetUpcomingEvents();
        List<Event> SearchEvents(string searchTerm);
        List<Event> GetEventsByLocation(string location);
        List<Event> GetEventsByHallId(int hallId);
        //void ValidateEvent(EventDto even);//לא יודעת אם צריך כי בכל מקרה זה private
    }
}
