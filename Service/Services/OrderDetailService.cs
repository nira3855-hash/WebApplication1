using AutoMapper;
using Repository.Entities;
using Repository.Interfaces;
using Service.Dto;
using Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Service.Services
{
    public class OrderDetailService : OrderDetailIService
    {
        private readonly IRepository<Event> eventRepository;
        private readonly IRepository<HallSeat> seatRepository;
        private readonly IRepository<OrderDetail> repository;
        private readonly IMapper mapper;

        public OrderDetailService(IRepository<OrderDetail> repository, IMapper mapper,
                                  IRepository<Event> eventR, IRepository<HallSeat> SeatR)
        {
            this.seatRepository = SeatR;
            this.eventRepository = eventR;
            this.repository = repository;
            this.mapper = mapper;
        }

        public OrderDetailDto AddToCartItem(OrderDetailCreateDto item)
        {
            var exists = repository.GetAll()
                .FirstOrDefault(o => o.EventID == item.EventID
                                  && o.HallSeatID == item.HallSeatID
                                  && o.Status != OrderStatus.Cancelled);

            if (exists != null)
                throw new Exception("Seat is already reserved or sold");

            double price = CalculatePrice(item.EventID, item.HallSeatID);

            var entity = mapper.Map<OrderDetail>(item);
            entity.PriceAtPurchase = price;
            entity.Status = OrderStatus.Reserved;
            entity.SelectAt = DateTime.Now;

            var saved = repository.AddItem(entity);
            return mapper.Map<OrderDetailDto>(saved);
        }

        public OrderDetailDto CompleteOrderItem(OrderDetailCreateDto item)
        {
           
            var exists = repository.GetAll()
                .FirstOrDefault(o => o.EventID == item.EventID
                                  && o.HallSeatID == item.HallSeatID
                                  && o.Status != OrderStatus.Cancelled);

            if (exists != null)
                throw new Exception("Seat is already reserved or sold");

            double price = CalculatePrice(item.EventID, item.HallSeatID);

            var entity = mapper.Map<OrderDetail>(item);
            entity.PriceAtPurchase = price;
            entity.Status = OrderStatus.Sold;
            entity.SelectAt = DateTime.Now;

            var saved = repository.AddItem(entity);
            return mapper.Map<OrderDetailDto>(saved);
        }

        public void DeleteItem(int id)
        {
            var order = repository.GetById(id);
            if (order == null)
                throw new NotImplementedException();

            repository.DeleteItem(id);
        }

        public double CalculatePrice(int eventId, int seatId)
        {
            var eventObj = eventRepository.GetById(eventId);
            var seatObj = seatRepository.GetById(seatId);

            if (eventObj == null || seatObj == null)
                throw new Exception("Event or Seat not found");

            return eventObj.BasePrice + seatObj.AddPrice;
        }

        public List<OrderDetailDto> GetAll()
        {
            return mapper.Map<List<OrderDetail>, List<OrderDetailDto>>(repository.GetAll());
        }

        public void CancelReservation(int userId, int orderDetailId)//מחיקה איבר מהסל
        {
            var item = repository.GetById(orderDetailId);
            if (item.UserID != userId || item.Status != OrderStatus.Reserved)
                throw new Exception("Cannot cancel");

            
            repository.DeleteItem(orderDetailId);
        }

        public OrderDetailDto GetById(int id)
        {
            var order = repository.GetById(id);
            if (order == null)
                throw new NotImplementedException();

            return mapper.Map<OrderDetail, OrderDetailDto>(order);
        }

        public void UpdateItem(int id, OrderDetailDto item)
        {
            var order = repository.GetById(id);
            if (order == null)
                throw new NotImplementedException();

            order.SelectAt = item.SelectAt;
            order.Status = item.Status;
            order.HallSeatID = item.HallSeatID;

            repository.UpdateItem(id, order);
        }
    }
}