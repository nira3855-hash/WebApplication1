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
    public class HallSeatService : IService<HallSeatDto>
    {
        private readonly IRepository<HallSeat> repository;
        private readonly IMapper mapper;

        public HallSeatService(IRepository<HallSeat> repository, IMapper mapper)
        {
            this.repository = repository;
            this.mapper = mapper;
        }

        public async Task<HallSeatDto> AddItemAsync(HallSeatDto item)
        {
            var entity = mapper.Map<HallSeatDto, HallSeat>(item);
            var added = await repository.AddItemAsync(entity);
            return mapper.Map<HallSeat, HallSeatDto>(added);
        }

        public async Task DeleteItemAsync(int id)
        {
            var hallSeat = await repository.GetByIdAsync(id);
            if (hallSeat == null)
                throw new NotImplementedException();

            await repository.DeleteItemAsync(id);
        }

        public async Task<List<HallSeatDto>> GetAllAsync()
        {
            var list = await repository.GetAllAsync();
            return mapper.Map<List<HallSeat>, List<HallSeatDto>>(list);
        }

        public async Task<HallSeatDto> GetByIdAsync(int id)
        {
            var hallSeat = await repository.GetByIdAsync(id);
            if (hallSeat == null)
                throw new NotImplementedException();

            return mapper.Map<HallSeat, HallSeatDto>(hallSeat);
        }

        public async Task UpdateItemAsync(int id, HallSeatDto item)
        {
            var hallSeat = await repository.GetByIdAsync(id);
            if (hallSeat == null)
                throw new NotImplementedException();

            hallSeat.TypeOfPlace = item.TypeOfPlace;
            hallSeat.SeatNumber = item.SeatNumber;
            hallSeat.RowNumber = item.RowNumber;
            hallSeat.AddPrice = item.AddPrice;

            await repository.UpdateItemAsync(id, hallSeat);
        }
    }
}