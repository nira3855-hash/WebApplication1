using AutoMapper;
using Repository.Entities;
using Repository.Interfaces;
using Repository.Repositories;
using Service.Dto;
using Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace Service.Services
{
    public class EventService: EventIService
    {
        private readonly EventIRepository eventRepository;
        private readonly IRepository<Producer> producerRepository;
        private readonly IRepository<Hall> hallRepository;
        private readonly IMapper mapper;
        public EventService(EventIRepository eventRepository,IRepository<Producer>producerRepository,IRepository<Hall> hallRepository, IMapper mapper)
        {
            this.eventRepository = eventRepository;
            this.producerRepository = producerRepository;
            this.hallRepository = hallRepository;
            this.mapper = mapper;
        }
        public EventDto AddEvent(EventDto item)
        {
            ValidateEvent(item);
            return mapper.Map<Event, EventDto>(eventRepository.AddItem(mapper.Map<EventDto, Event>(item)));
        }

        public void DeleteEvent(int id)
        {
            var existingEvent = eventRepository.GetById(id);

            if (existingEvent == null)
                throw new ArgumentException("האירוע לא קיים.");
            eventRepository.DeleteItem(id);
        }

        public List<EventDto> GetAllEvents()
        {
            return mapper.Map<List<Event>, List<EventDto>>(eventRepository.GetAll());
        }

        public EventDto GetEventById(int id)
        {
            var even = eventRepository.GetById(id);

            if (even == null)
                throw new ArgumentException("האירוע לא קיים.");
            return mapper.Map<Event, EventDto>(even);
        }

        public void UpdateEvent(int id, EventDto item)
        {
            var even = eventRepository.GetById(id);

            if (even == null)
                throw new ArgumentException("האירוע לא קיים.");

            ValidateEvent(item);

            //even.Title = item.Title;
            //even.EventDate = item.EventDate;
            //even.Location = item.Location;
            //even.BasePrice = item.BasePrice;
            //even.Describe = item.Describe;
            //even.HallID = item.HallID;
            eventRepository.UpdateItem (id,mapper.Map<EventDto, Event> (item));
        }

        public List<Event> GetEventsByProducerId(int producerId)
        {
            var producer = producerRepository.GetById(producerId);
            if (producer == null)
            {
                throw new ArgumentException("המפיק לא קיים.");
            }
            return eventRepository.GetByProducerId(producerId);
        }

        public List<Event> GetEventsByDate(DateTime date)
        {
            return eventRepository.GetByDate(date);
        }

        public List<Event> GetUpcomingEvents()
        {
            return eventRepository.GetUpcomingEvents();
        }

        public List<Event> SearchEvents(string searchTerm)
        {
            return eventRepository.Search(searchTerm);
        }

        public List<Event> GetEventsByLocation(string location)
        {
            return eventRepository.GetByLocation(location);
        }

        public List<Event> GetEventsByHallId(int hallId)
        {
            var hall = hallRepository.GetById(hallId);
            if (hall == null)
            {
                throw new ArgumentException("האולם לא קיים.");
            }
            return eventRepository.GetByHallId(hallId);
        }

        private void ValidateEvent(EventDto even)
        { 
            if (even.EventDate < DateTime.Now)
            {
                throw new ArgumentException("תאריך האירוע חייב להיות בעתיד.");
            }
            if (even.BasePrice < 0)
            {
                throw new ArgumentException("המחיר הבסיסי חייב להיות חיובי.");
            }
            if (string.IsNullOrWhiteSpace(even.Location))
            {
                throw new ArgumentException("המיקום אינו יכול להיות ריק.");
            }
            var producer = producerRepository.GetById(even.ProducerID);
            if (producer == null)
            {
                throw new ArgumentException("המפיק לא קיים.");
            }
            var hall = hallRepository.GetById(even.HallID);
            if (hall == null)
            {
                throw new ArgumentException("האולם לא קיים.");
            }
            //if (!string.IsNullOrEmpty(even.ImageUrl) && !Uri.IsWellFormedUriString(event.ImageUrl, UriKind.Absolute))
            //{
            //    throw new ArgumentException("כתובת ה-URL לתמונה אינה תקינה.");
            //}
        }


        
        
    }
}
