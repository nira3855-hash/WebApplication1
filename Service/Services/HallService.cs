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
    public class HallService : IService<HallDto>
    {
        private readonly IRepository<Hall> repository;
        private readonly IMapper mapper;

        public HallService(IRepository<Hall> repository, IMapper mapper)
        {
            this.repository = repository;
            this.mapper = mapper;
        }

        public async Task<HallDto> AddItemAsync(HallDto item)
        {
            var entity = mapper.Map<HallDto, Hall>(item);
            var added = await repository.AddItemAsync(entity);
            return mapper.Map<Hall, HallDto>(added);
        }

        public async Task DeleteItemAsync(int id)
        {
            var hall = await repository.GetByIdAsync(id);
            if (hall == null)
                throw new NotImplementedException();

            await repository.DeleteItemAsync(id);
        }

        public async Task<List<HallDto>> GetAllAsync()
        {
            var list = await repository.GetAllAsync();
            return mapper.Map<List<Hall>, List<HallDto>>(list);
        }

        public async Task<HallDto> GetByIdAsync(int id)
        {
            var hall = await repository.GetByIdAsync(id);
            if (hall == null)
                throw new NotImplementedException();

            return mapper.Map<Hall, HallDto>(hall);
        }

        public async Task UpdateItemAsync(int id, HallDto item)
        {
            var hall = await repository.GetByIdAsync(id);
            if (hall == null)
                throw new NotImplementedException();

            hall.name = item.name;
            hall.location = item.location;
            hall.numOfSeats = item.numOfSeats;
            hall.shape = item.shape;

            await repository.UpdateItemAsync(id, hall);
        }
    }
}