using AutoMapper;
using Repository.Entities;
using Repository.Interfaces;
using Service.Dto;
using Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace Service.Services
{
    public class EventService: IService<EventDto>
    {
        private readonly IRepository<Event> repository;
        private readonly IMapper mapper;
        public EventService(IRepository<Event> repository, IMapper mapper)
        {
            this.repository = repository;
            this.mapper = mapper;
        }
        public EventDto AddItem(EventDto item)
        {
            return mapper.Map<Event, EventDto>(repository.AddItem(mapper.Map<EventDto, Event>(item)));
        }

        public void DeleteItem(int id)
        {
            var even = repository.GetById(id);

            if (even == null)
                throw new NotImplementedException();
            repository.DeleteItem(id);
        }

        public List<EventDto> GetAll()
        {
            return mapper.Map<List<Event>, List<EventDto>>(repository.GetAll());
        }

        public EventDto GetById(int id)
        {
            var even = repository.GetById(id);

            if (even == null)
                throw new NotImplementedException();
            return mapper.Map<Event, EventDto>(even);
        }

        public void UpdateItem(int id, EventDto item)
        {
            var even = repository.GetById(id);

            if (even == null)
                throw new NotImplementedException();

            even.Title = item.Title;
            even.EventDate = item.EventDate;
            even.Location = item.Location;
            even.BasePrice = item.BasePrice;
            even.Describe = item.Describe;
            even.HallID = item.HallID;
            repository.UpdateItem(id, even);
        }
    }
}
