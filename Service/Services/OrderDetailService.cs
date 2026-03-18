using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Repository.Entities;
using Repository.Interfaces;
using Repository.Repositories;
using Service.Dto;
using Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Service.Services
{
    public class OrderDetailService : OrderDetailIService
    {
        private readonly EventIRepository eventRepository;
        private readonly IRepository<HallSeat> seatRepository;
        private readonly OrderDetailIRepository repository;
        private readonly IMapper mapper;

        public OrderDetailService(OrderDetailIRepository repository,
                                    IMapper mapper,
                               EventIRepository eventR,
                          IRepository<HallSeat> seatR)
        {
            this.seatRepository = seatR;
            this.eventRepository = eventR;
            this.repository = repository;
            this.mapper = mapper;
        }

        public async Task<OrderDetailDto> AddToCartItemAsync(OrderDetailCreateDto item)
        {
            var exists = (await repository.GetAllAsync())
                .FirstOrDefault(o => o.EventID == item.EventID
                                  && o.HallSeatID == item.HallSeatID
                                  && o.Status != OrderStatus.Cancelled);

            if (exists != null)
                throw new Exception("Seat is already reserved or sold");

            double price = await CalculatePriceAsync(item.EventID, item.HallSeatID);

            var entity = mapper.Map<OrderDetail>(item);
            entity.PriceAtPurchase = price;
            entity.Status = OrderStatus.Reserved;
            entity.SelectAt = DateTime.Now;

            var saved = await repository.AddItemAsync(entity);
            return mapper.Map<OrderDetailDto>(saved);
        }

        public async Task<List<UserOrdersByEventDto>> GetCartGroupedByEvent(int userId)
        {
            var cartItems = await repository.GetCartAsync(userId);

            var groupedTasks = cartItems
                .GroupBy(o => o.EventID)
                .Select(async g => new UserOrdersByEventDto
                {
                    Event = g.ToList().First().Event,
                    Seats = g.Select(o => mapper.Map<OrderDetailDto>(o)).ToList()
                });

            var grouped = await Task.WhenAll(groupedTasks);

            return grouped.ToList();
        }

        public async Task<List<UserOrdersByEventDto>> GetRealOrdersGroupedByEvent(int userId)
        {
            var cartItems = await repository.GetRealOrdersAsync(userId);

            var groupedTasks = cartItems
                 .GroupBy(o => o.EventID)
                 .Select(async g => new UserOrdersByEventDto
                 {
                     Event = g.ToList().First().Event,
                     Seats = g.Select(o => mapper.Map<OrderDetailDto>(o)).ToList()
                 });

            var grouped = await Task.WhenAll(groupedTasks);

            return grouped.ToList();
        }

        public async Task<OrderDetailDto> CompleteOrderItemAsync(OrderDetailCreateDto item)
        {
            var exists = (await repository.GetAllAsync())
                .FirstOrDefault(o => o.EventID == item.EventID
                                  && o.HallSeatID == item.HallSeatID
                                  && o.Status != OrderStatus.Cancelled);

            if (exists != null)
                throw new Exception("Seat is already reserved or sold");

            double price = await CalculatePriceAsync(item.EventID, item.HallSeatID);

            var entity = mapper.Map<OrderDetail>(item);
            entity.PriceAtPurchase = price;
            entity.Status = OrderStatus.Sold;
            entity.SelectAt = DateTime.Now;

            var saved = await repository.AddItemAsync(entity);
            return mapper.Map<OrderDetailDto>(saved);
        }

        public async Task DeleteItemAsync(int id)
        {
            var order = await repository.GetByIdAsync(id);
            if (order == null)
                throw new NotImplementedException();

            await repository.DeleteItemAsync(id);
        }

        public async Task<double> CalculatePriceAsync(int eventId, int seatId)
        {
            var eventObj = await eventRepository.GetByIdAsync(eventId);
            var seatObj = await seatRepository.GetByIdAsync(seatId);

            if (eventObj == null || seatObj == null)
                throw new Exception("Event or Seat not found");

            return eventObj.BasePrice + seatObj.AddPrice;
        }

        public async Task<List<OrderDetailDto>> GetAllAsync()
        {
            var list = await repository.GetAllAsync();
            return mapper.Map<List<OrderDetail>, List<OrderDetailDto>>(list);
        }

        public async Task CancelReservationAsync(int userId, int orderDetailId)
        {
            var item = await repository.GetByIdAsync(orderDetailId);
            if (item.UserID != userId || item.Status != OrderStatus.Reserved)
                throw new Exception("Cannot cancel");

            await repository.DeleteItemAsync(orderDetailId);
        }

        public async Task<List<OrderDetailDto>> GetByUserIdAsync(int id)
        {
            var order = await repository.GetByUserIdAsync(id);
            if (order == null)
                throw new NotImplementedException();

            return mapper.Map<List<OrderDetail>, List<OrderDetailDto>>(order);
        }

        public async Task<OrderDetailDto> GetByIdAsync(int id)
        {
            var order = await repository.GetByIdAsync(id);
            if (order == null)
                throw new NotImplementedException();

            return mapper.Map<OrderDetail, OrderDetailDto>(order);
        }

        public async Task<List<OrderDetailDto>> GetOrdersByEventIdAsync(int eventId)
        {
            var orders = await repository.GetByEventIdAsync(eventId);
            return mapper.Map<List<OrderDetail>, List<OrderDetailDto>>(orders);
        }

        public async Task<List<OrderDetailDto>> AddMultipleToCartAsync(CompleteMultipleSeatsDto dto)
        {
            var result = new List<OrderDetailDto>();

            var eventObj = await eventRepository.GetByIdAsync(dto.EventId);
            if (eventObj == null)
                throw new Exception("Event not found");

            if (dto.HallSeatIds == null || !dto.HallSeatIds.Any())
                throw new Exception("No seats selected");

            var bookedSeats = await repository.GetBookedSeatsByEvent(dto.EventId, dto.HallSeatIds);

            if (bookedSeats.Any())
                throw new Exception($"Seats already reserved: {string.Join(",", bookedSeats)}");

            foreach (var seatId in dto.HallSeatIds)
            {
                var seatObj = await seatRepository.GetByIdAsync(seatId);
                if (seatObj == null)
                    throw new Exception($"Seat {seatId} not found");

                double price = await CalculatePriceAsync(dto.EventId, seatId);

                var entity = new OrderDetail
                {
                    EventID = dto.EventId,
                    HallSeatID = seatId,
                    UserID = dto.UserId,
                    Status = OrderStatus.Reserved,
                    PriceAtPurchase = price,
                    SelectAt = DateTime.Now
                };

                var saved = await repository.AddItemAsync(entity);
                result.Add(mapper.Map<OrderDetailDto>(saved));
            }

            return result;
        }

        public async Task<List<OrderDetailDto>> CompleteMultipleOrderAsync(CompleteMultipleSeatsDto dto)
        {
            if (dto.HallSeatIds == null || !dto.HallSeatIds.Any())
                throw new Exception("No seats selected");

            var eventObj = await eventRepository.GetByIdAsync(dto.EventId);
            if (eventObj == null)
                throw new Exception("Event not found");

            var userCartSeats = await repository.GetReservedSeatsByUser(dto.UserId, dto.EventId);
            var allBookedSeats = await repository.GetBookedSeatsByEvent(dto.EventId, dto.HallSeatIds);
            var missingInCart = dto.HallSeatIds.Except(userCartSeats.Select(s => s.HallSeatID)).ToList();
            var partiallyBooked = dto.HallSeatIds.Intersect(allBookedSeats).ToList();

            if (partiallyBooked.Any())
                throw new Exception($"Some seats are already reserved or sold: {string.Join(",", partiallyBooked)}");

            bool allInCart = !missingInCart.Any();
            bool allFree = missingInCart.Count == dto.HallSeatIds.Count;

            if (!allInCart && !allFree)
                throw new Exception("Cannot mix seats in cart and free seats. Transaction aborted.");

            var result = new List<OrderDetailDto>();

            if (allInCart)
            {
                foreach (var seatId in dto.HallSeatIds)
                {
                    var cartItem = userCartSeats.First(s => s.HallSeatID == seatId);
                    cartItem.Status = OrderStatus.Sold;
                    cartItem.PriceAtPurchase = await CalculatePriceAsync(dto.EventId, seatId);
                    cartItem.SelectAt = DateTime.Now;

                    var updated = await repository.UpdateItemsAsyncs(new List<OrderDetail> { cartItem });
                    result.Add(mapper.Map<OrderDetailDto>(updated.First()));
                }
            }
            else if (allFree)
            {
                foreach (var seatId in dto.HallSeatIds)
                {
                    var price = await CalculatePriceAsync(dto.EventId, seatId);

                    var seatEntity = new OrderDetail
                    {
                        EventID = dto.EventId,
                        HallSeatID = seatId,
                        UserID = dto.UserId,
                        Status = OrderStatus.Sold,
                        PriceAtPurchase = price,
                        SelectAt = DateTime.Now
                    };

                    var saved = await repository.AddItemAsync(seatEntity);
                    result.Add(mapper.Map<OrderDetailDto>(saved));
                }
            }

            return result;
        }

        public async Task UpdateItemAsync(int id, OrderDetailDto item)
        {
            var order = await repository.GetByIdAsync(id);
            if (order == null)
                throw new NotImplementedException();

            order.SelectAt = item.SelectAt;
            order.Status = item.Status;
            order.HallSeatID = item.HallSeatID;

            await repository.UpdateItemAsync(id, order);
        }
    }
}