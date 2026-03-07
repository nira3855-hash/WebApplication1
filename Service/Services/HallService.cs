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
    public class HallService: IService<HallDto>
    {
        private readonly IRepository<Hall> repository;
        private readonly IMapper mapper;
        public HallService(IRepository<Hall> repository, IMapper mapper)
        {
            this.repository = repository;
            this.mapper = mapper;
        }
        public HallDto AddItem(HallDto item)
        {
            return mapper.Map<Hall, HallDto>(repository.AddItem(mapper.Map<HallDto, Hall>(item)));
        }

        public void DeleteItem(int id)
        {
            var hall = repository.GetById(id);

            if (hall == null)
                throw new NotImplementedException();
            repository.DeleteItem(id);
        }

        public List<HallDto> GetAll()
        {
            return mapper.Map<List<Hall>, List<HallDto>>(repository.GetAll());
        }

        public HallDto GetById(int id)
        {
            var hall = repository.GetById(id);
            if (hall == null)
                 throw new NotImplementedException();
            return mapper.Map<Hall, HallDto>(hall);

        }

        public void UpdateItem(int id, HallDto item)
        {
            var hall = repository.GetById(id);
            if (hall == null)
                throw new NotImplementedException();
            hall.name = item.name;
            hall.location = item.location;
            hall.numOfSeats = item.numOfSeats;
            hall.shape = item.shape;
            repository.UpdateItem(id, hall);
        }
    }
}
