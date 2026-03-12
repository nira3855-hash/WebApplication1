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
    public class HallSeatService : HallSeatIService
    {
        private readonly HallSeatIRepository repository;
        private readonly IMapper mapper;

        public HallSeatService(HallSeatIRepository repository, IMapper mapper)
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
                throw new ArgumentException("המקום לא קיים.");

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
                throw new ArgumentException("המקום לא קיים.");

            return mapper.Map<HallSeat, HallSeatDto>(hallSeat);
        }
        public async Task<List<HallSeatDto>> GetByHallIdAsync(int HallId)
        {
            var hallSeat = await repository.GetByHallIdAsync(HallId);
            if (hallSeat == null)
                throw new ArgumentException("המקום לא קיים.");

            return mapper.Map< List<HallSeat>, List<HallSeatDto>>(hallSeat);
        }
        public async Task UpdateItemAsync(int id, HallSeatDto item)
        {
            var hallSeat = await repository.GetByIdAsync(id);
            if (hallSeat == null)
                throw new ArgumentException("המקום לא קיים.");

            hallSeat.TypeOfPlace = item.TypeOfPlace;
            hallSeat.SeatNumber = item.SeatNumber;
            hallSeat.RowNumber = item.RowNumber;
            hallSeat.AddPrice = item.AddPrice;

            await repository.UpdateItemAsync(id, hallSeat);
        }
    }
}