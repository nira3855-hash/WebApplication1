using Repository.Entities;
using Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Repositories
{
    public class EventRepository : EventIRepository
    {
        private readonly IContext _context;
        public EventRepository(IContext context)
        {
            this._context = context;
        }
        public Event AddItem(Event item)
        {
            _context.Events.Add(item);

            _context.save();
            return item;
        }

        public void DeleteItem(int id)
        {
            _context.Events.Remove(GetById(id));
            _context.save();
        }

        public List<Event> GetAll()
        {
            return _context.Events.ToList();
        }

        public Event GetById(int id)
        {
            return _context.Events.FirstOrDefault(x => x.Id == id);
        }

        public Event UpdateItem(int id, Event item)
        {
            var Event = GetById(id);
            Event.ProducerID= item.ProducerID;
            Event.Producer= item.Producer;
            Event.Title = item.Title;
            Event.EventDate = item.EventDate;
            Event.HallID = item.HallID;
            Event.BasePrice = item.BasePrice;
            Event.ImageUrl = item.ImageUrl;
            
            _context.save();
            return Event;
        }

        public List<Event> GetByProducerId(int producerId)
        {
            return _context.Events.Where(e => e.ProducerID == producerId).ToList();
        }

        public List<Event> GetByDate(DateTime date)
        {
            return _context.Events.Where(e => e.EventDate.Date == date.Date).ToList();
        }

        public List<Event> GetUpcomingEvents()
        {
            return _context.Events.Where(e => e.EventDate > DateTime.Now).ToList();
        }

        public List<Event> Search(string searchTerm)
        {
            return _context.Events.Where(e => e.Title.Contains(searchTerm) || e.Describe.Contains(searchTerm)).ToList();
        }

        public List<Event> GetByLocation(string location)
        {
            return _context.Events.Where(e => e.Location.Equals(location, StringComparison.OrdinalIgnoreCase)).ToList();
        }

        public List<Event> GetByHallId(int hallId)
        {
            return _context.Events.Where(e => e.HallID == hallId).ToList();
        }

    }
}
