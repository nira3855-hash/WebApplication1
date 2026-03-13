using AutoMapper;
using Repository.Entities;
using Repository.Interfaces;
using Repository.Repositories;
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
        private readonly HallSeatIRepository hallSeatRepository;
        private readonly OrderDetailIRepository orderDetailRepository;
        private readonly IMapper mapper;

        public EventService(
            EventIRepository eventRepository,
            IRepository<Producer> producerRepository,
            IRepository<Hall> hallRepository,
            HallSeatIRepository hallSeatRepository,
            OrderDetailIRepository orderDetailRepository,
            IMapper mapper)
        {
            this.eventRepository = eventRepository;
            this.producerRepository = producerRepository;
            this.hallRepository = hallRepository;
            this.hallSeatRepository = hallSeatRepository;
            this.orderDetailRepository = orderDetailRepository;
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

        public async Task<List<HallSeatDto>> FindBestSeatsAsync(int eventId, int count, bool preferPremium)
        {
            //  שליפת הנתונים מה-Repositories (עם התיקון ל-GetAll שדיברנו עליו)
            var eventObj = await eventRepository.GetByIdAsync(eventId);
            if (eventObj == null) return new List<HallSeatDto>();

            var allSeatsInSystem = await hallSeatRepository.GetAllAsync();
            var roomSeats = allSeatsInSystem.Where(s => s.HallID == eventObj.HallID).ToList();

            var allOrders = await orderDetailRepository.GetAllAsync();
            var occupiedIds = allOrders
                .Where(o => o.EventID == eventId && (o.Status == OrderStatus.Reserved || o.Status == OrderStatus.Sold))
                .Select(o => o.HallSeatID).ToHashSet();

            //  סינון מושבים פנויים בלבד
            var freeSeats = roomSeats.Where(s => !occupiedIds.Contains(s.Id)).ToList();

            //  חלוקה לקבוצות לפי סוג (ממוינים מראש לפי שורה וכיסא)
            var premiumSeats = freeSeats
                .Where(s => s.TypeOfPlace != "Regular")
                .OrderBy(s => s.RowNumber).ThenBy(s => s.SeatNumber).ToList();

            var regularSeats = freeSeats
                .Where(s => s.TypeOfPlace == "Regular")
                .OrderBy(s => s.RowNumber).ThenBy(s => s.SeatNumber).ToList();

            List<HallSeat> finalSelection = null;




            if (preferPremium)
            {
                // א. ניסיון במיוחדים: קודם שורה אחת, ואז בלוק (חצי-חצי)
                finalSelection = FindSequentialInRow(premiumSeats, count);
                if (finalSelection == null && count > 1)
                    finalSelection = FindBestBlock(premiumSeats, count);

                // ב. אם לא מצאנו במיוחדים - ננסה ברגילים (קודם שורה, אז בלוק)
                if (finalSelection == null)
                {
                    finalSelection = FindSequentialInRow(regularSeats, count);
                    if (finalSelection == null && count > 1)
                        finalSelection = FindBestBlock(regularSeats, count);
                }
            }
            else
            {
                // ג. המשתמש רוצה רגיל - נחפש רצף בשורה או בלוק רק ברגילים
                finalSelection = FindSequentialInRow(regularSeats, count);
                if (finalSelection == null && count > 1)
                    finalSelection = FindBestBlock(regularSeats, count);
            }

            // ד. "המוצא האחרון" - אם לא נמצא שום רצף הגיוני
            if (finalSelection == null)
            {
                finalSelection = freeSeats
                    .OrderBy(s => s.RowNumber)
                    .ThenBy(s => s.SeatNumber)
                    .Take(count)
                    .ToList();
            }

            return mapper.Map<List<HallSeatDto>>(finalSelection);
        }

        private List<HallSeat> FindSequentialInRow(List<HallSeat> seats, int count)
        {
            // קיבוץ המושבים לפי מספר שורה
            var rows = seats.GroupBy(s => s.RowNumber);

            foreach (var row in rows)
            {
                // סידור המושבים בשורה לפי המספר שלהם (למשל 1, 2, 4, 5...)
                var rowSeats = row.OrderBy(s => s.SeatNumber).ToList();

                // אם אין מספיק מושבים פנויים בשורה הזו, דלג
                if (rowSeats.Count < count) continue;

                for (int i = 0; i <= rowSeats.Count - count; i++)
                {
                    // לוקחים קבוצה של כיסאות בגודל שהמשתמש ביקש
                    var candidate = rowSeats.Skip(i).Take(count).ToList();

                    // בדיקה: האם ההפרש בין הכיסא האחרון לראשון תואם לכמות?
                    // (למשל: כיסא 10 פחות כיסא 8 שווה 2, זה אומר שהם 8, 9, 10 - רצף מושלם)
                    if (candidate.Last().SeatNumber - candidate.First().SeatNumber == count - 1)
                    {
                        return candidate; // מצאנו רצף!
                    }
                }
            }
            return null; // לא נמצא רצף באף שורה
        }

        private List<HallSeat> FindBestBlock(List<HallSeat> seats, int count)
        {
            // 1. ננסה לחלק את הקבוצה לשתי שורות (למשל 4 אנשים -> 2 ו-2)
            int firstRowSize = (int)Math.Ceiling(count / 2.0);
            int secondRowSize = count - firstRowSize;

            var rows = seats.GroupBy(s => s.RowNumber).OrderBy(r => r.Key).ToList();

            for (int i = 0; i < rows.Count - 1; i++)
            {
                var currentRow = rows[i].OrderBy(s => s.SeatNumber).ToList();
                var nextRow = rows[i + 1].OrderBy(s => s.SeatNumber).ToList();

                // נחפש רצף בשורה הנוכחית עבור החצי הראשון
                for (int j = 0; j <= currentRow.Count - firstRowSize; j++)
                {
                    var firstPart = currentRow.Skip(j).Take(firstRowSize).ToList();

                    // בדיקה שהחלק הראשון רציף
                    if (firstPart.Last().SeatNumber - firstPart.First().SeatNumber == firstRowSize - 1)
                    {
                        // עכשיו נחפש בשורה הבאה רצף באותם מספרי כיסאות בדיוק
                        var firstSeatNum = firstPart.First().SeatNumber;
                        var secondPart = nextRow
                            .Where(s => s.SeatNumber >= firstSeatNum && s.SeatNumber < firstSeatNum + secondRowSize)
                            .ToList();

                        if (secondPart.Count == secondRowSize &&
                            (secondPart.Count == 1 || secondPart.Last().SeatNumber - secondPart.First().SeatNumber == secondRowSize - 1))
                        {
                            return firstPart.Concat(secondPart).ToList();
                        }
                    }
                }
            }
            return null; // לא נמצא בלוק זוגי צמוד
        }
    }
}