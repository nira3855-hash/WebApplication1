using AutoMapper;
using Repository.Entities;
using Repository.Interfaces;
using Service.Dto;
using Service.Interface;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service.Services
{
    public class EventService : EventIService
    {
        private readonly EventIRepository eventRepository;
        private readonly IRepository<Producer> producerRepository;
        private readonly IRepository<Hall> hallRepository;
        private readonly IMapper mapper;

        public EventService(
            EventIRepository eventRepository,
            IRepository<Producer> producerRepository,
            IRepository<Hall> hallRepository,
            IMapper mapper)
        {
            this.eventRepository = eventRepository;
            this.producerRepository = producerRepository;
            this.hallRepository = hallRepository;
            this.mapper = mapper;
        }

        public async Task<EventDto> AddEventAsync(EventDto item)
        {
            ValidateEvent(item);
            var entity = mapper.Map<EventDto, Event>(item);
            var added = await eventRepository.AddItemAsync(entity);
            return mapper.Map<Event, EventDto>(added);
        }

        public async Task DeleteEventAsync(int id)
        {
            var existingEvent = await eventRepository.GetByIdAsync(id);
            if (existingEvent == null)
                throw new ArgumentException("האירוע לא קיים.");

            await eventRepository.DeleteItemAsync(id);
        }

        public async Task<List<EventDto>> GetAllEventsAsync()
        {
            var events = await eventRepository.GetAllAsync();
            return mapper.Map<List<Event>, List<EventDto>>(events);
        }

        public async Task<EventDto> GetEventByIdAsync(int id)
        {
            var even = await eventRepository.GetByIdAsync(id);
            if (even == null)
                throw new ArgumentException("האירוע לא קיים.");

            return mapper.Map<Event, EventDto>(even);
        }

        public async Task UpdateEventAsync(int id, EventDto item)
        {
            var even = await eventRepository.GetByIdAsync(id);
            if (even == null)
                throw new ArgumentException("האירוע לא קיים.");

            ValidateEvent(item);
            await eventRepository.UpdateItemAsync(id, mapper.Map<EventDto, Event>(item));
        }

        public async Task<List<Event>> GetEventsByProducerIdAsync(int producerId)
        {
            var producer = await producerRepository.GetByIdAsync(producerId);
            if (producer == null)
                throw new ArgumentException("המפיק לא קיים.");

            return await eventRepository.GetByProducerIdAsync(producerId);
        }

        public async Task<List<Event>> GetEventsByDateAsync(DateTime date)
        {
            return await eventRepository.GetByDateAsync(date);
        }

        public async Task<List<Event>> GetUpcomingEventsAsync()
        {
            return await eventRepository.GetUpcomingEventsAsync();
        }

        public async Task<List<Event>> SearchEventsAsync(string searchTerm)
        {
            return await eventRepository.SearchAsync(searchTerm);
        }

        public async Task<List<Event>> GetEventsByLocationAsync(string location)
        {
            return await eventRepository.GetByLocationAsync(location);
        }

        public async Task<List<Event>> GetEventsByHallIdAsync(int hallId)
        {
            var hall = await hallRepository.GetByIdAsync(hallId);
            if (hall == null)
                throw new ArgumentException("האולם לא קיים.");

            return await eventRepository.GetByHallIdAsync(hallId);
        }

        private void ValidateEvent(EventDto even)
        {
            if (even.EventDate < DateTime.Now)
                throw new ArgumentException("תאריך האירוע חייב להיות בעתיד.");

            if (even.BasePrice < 0)
                throw new ArgumentException("המחיר הבסיסי חייב להיות חיובי.");

            if (string.IsNullOrWhiteSpace(even.Location))
                throw new ArgumentException("המיקום אינו יכול להיות ריק.");

            var producer = producerRepository.GetByIdAsync(even.ProducerID).Result;
            if (producer == null)
                throw new ArgumentException("המפיק לא קיים.");

            var hall = hallRepository.GetByIdAsync(even.HallID).Result;
            if (hall == null)
                throw new ArgumentException("האולם לא קיים.");
        }
    }
}