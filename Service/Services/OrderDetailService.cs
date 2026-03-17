using AutoMapper;
using Repository.Entities;
using Repository.Interfaces;
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
                  IRepository<HallSeat> seatR){
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
            // שליפת כל ההזמנות (כולל שמורות וכולל מכורות) עבור האירוע
            var orders = await repository.GetByEventIdAsync(eventId);

            // מיפוי לרשימת Dto
            return mapper.Map<List<OrderDetail>, List<OrderDetailDto>>(orders);
        }

        public async Task<List<OrderDetailDto>> AddMultipleToCartAsync(CompleteMultipleSeatsDto dto)
        {
            var result = new List<OrderDetailDto>();

            // בדיקה אם האירוע קיים
            var eventObj = await eventRepository.GetByIdAsync(dto.EventId);
            if (eventObj == null)
                throw new Exception("Event not found");

            // בדיקה אם נבחרו מושבים
            if (dto.HallSeatIds == null || !dto.HallSeatIds.Any())
                throw new Exception("No seats selected");

            // בדיקה אילו מושבים כבר תפוסים
            var bookedSeats = await repository.GetBookedSeatsByEvent(dto.EventId, dto.HallSeatIds);

            if (bookedSeats.Any())
                throw new Exception($"Seats already reserved: {string.Join(",", bookedSeats)}");

            foreach (var seatId in dto.HallSeatIds)
            {
                // בדיקה אם המושב קיים
                var seatObj = await seatRepository.GetByIdAsync(seatId);
                if (seatObj == null)
                    throw new Exception($"Seat {seatId} not found");

                // חישוב מחיר
                double price = eventObj.BasePrice + seatObj.AddPrice;

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

        /// מוסיף מספר מושבים להזמנה לאירוע בצורה אטומית
        public async Task<List<OrderDetailDto>> CompleteMultipleOrderAsync(CompleteMultipleSeatsDto dto)
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

                double price = eventObj.BasePrice + seatObj.AddPrice;

                var entity = new OrderDetail
                {
                    EventID = dto.EventId,
                    HallSeatID = seatId,
                    UserID = dto.UserId,
                    Status = OrderStatus.Sold,
                    PriceAtPurchase = price,
                    SelectAt = DateTime.Now
                };

                var saved = await repository.AddItemAsync(entity);

                result.Add(mapper.Map<OrderDetailDto>(saved));
            }

            return result;
        }
        //public async Task AddSeatsToOrderAsync(int eventId, List<int> seatIds, int userId)
        //{
        //    // בדיקה אילו מושבים תפוסים
        //    var bookedSeats = await repository.GetBookedSeatsByEvent(eventId, seatIds);

        //    var availableSeats = seatIds.Except(bookedSeats).ToList();

        //    if (!availableSeats.Any())
        //        throw new Exception("כל המושבים שבחרת כבר תפוסים.");

        //    foreach (var seatId in availableSeats)
        //    {
        //        await repository.AddItemAsync(new OrderDetail
        //        {
        //            EventID = eventId,
        //            HallSeatID = seatId,
        //            UserID = userId,
        //            Status = OrderStatus.Sold,
        //            PriceAtPurchase = 0,
        //            SelectAt = DateTime.UtcNow
        //        });
        //    }
        //}

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