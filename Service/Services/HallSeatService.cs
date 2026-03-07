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
    public class HallSeatService: IService<HallSeatDto>
    {
        private readonly IRepository<HallSeat> repository;
        private readonly IMapper mapper;
        public HallSeatService(IRepository<HallSeat> repository, IMapper mapper)
        {
            this.repository = repository;
            this.mapper = mapper;
        }
        public HallSeatDto AddItem(HallSeatDto item)
        {
            return mapper.Map<HallSeat, HallSeatDto>(repository.AddItem(mapper.Map<HallSeatDto, HallSeat>(item)));
        }

        public void DeleteItem(int id)
        {
            var hallSeat = repository.GetById(id);

            if (hallSeat == null)
                throw new NotImplementedException();
            repository.DeleteItem(id);
        }

        public List<HallSeatDto> GetAll()
        {
            return mapper.Map<List<HallSeat>, List<HallSeatDto>>(repository.GetAll());
        }

        public HallSeatDto GetById(int id)
        {
            var hallSeat = repository.GetById(id);

            if (hallSeat == null)
                throw new NotImplementedException();
            return mapper.Map<HallSeat, HallSeatDto>(hallSeat);
        }

        public void UpdateItem(int id, HallSeatDto item)
        {
            var hallSeat = repository.GetById(id);

            if (hallSeat == null)
                throw new NotImplementedException();
            hallSeat.TypeOfPlace = item.TypeOfPlace;
            hallSeat.SeatNumber = item.SeatNumber;
            hallSeat.RowNumber = item.RowNumber;
            hallSeat.AddPrice=item.AddPrice;

            repository.UpdateItem(id, hallSeat);
        }
    }
}
