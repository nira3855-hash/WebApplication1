using Repository.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Interfaces
{
    public interface EventIRepository
    {
        List<Event> GetAll();
        Event GetById(int id);
        Event AddItem(Event item);
        Event UpdateItem(int id, Event item);
        void DeleteItem(int id);
        List<Event> GetByProducerId(int producerId);
        List<Event> GetByDate(DateTime date);
        List<Event> GetUpcomingEvents();
        List<Event> Search(string searchTerm);
        List<Event> GetByLocation(string location);
        List<Event> GetByHallId(int hallId);
    }
}
