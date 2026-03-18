using AutoMapper;
using Repository.Entities;
using Repository.Interfaces;
using Repository.Repositories;
using Service.Dto;
using Service.Interface;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using static System.Formats.Asn1.AsnWriter;

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
           
            // 1. בדיקה אם האירוע קיים
            var existingEvent = await eventRepository.GetByIdAsync(id);
            if (existingEvent == null)
                throw new ArgumentException("האירוע לא קיים.");

            // 2. בדיקה אם יש הזמנות לאירוע הזה (כוללReserved ו-Sold)
            var orders = await orderDetailRepository.GetByEventIdAsync(id);

            // אם הרשימה לא ריקה - סימן שיש כרטיסים (בסל או שנקנו)
            if (orders != null && orders.Any())
            {
                throw new InvalidOperationException("לא ניתן למחוק את האירוע כיוון שישנן הזמנות פעילות או כרטיסים שנרכשו.");
            }

            // 3. אם הכל תקין - מוחקים
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

        public async Task<List<ProducerEventDto>> GetEventsByProducerIdAsync(int producerId)
        {
            // 1. שליפת האירועים מה-DB
            var eventsList = await eventRepository.GetEventsByProducerIdAsync(producerId);

            // 2. מיפוי ל-DTO החדש (היורש)
            var dtos = mapper.Map<List<Event>, List<ProducerEventDto>>(eventsList);

            // 3. מילוי נתוני המכירות לכל אירוע
            foreach (var dto in dtos)
            {
                var orders = await orderDetailRepository.GetByEventIdAsync(dto.Id);
                // סופרים רק הזמנות שאינן מבוטלות (Sold או Reserved)
                dto.TicketsSold = orders.Count(o => o.Status != OrderStatus.Cancelled);
            }

            return dtos;
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

        public async Task<List<HallSeatDto>> FindBestSeatsAsync(BestSeatsForEvent dto)
        {
            int eventId = dto.eventId;
            int count = dto.count;
            bool preferPremium = dto.preferPremium;
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
                .Where(s => s.TypeOfPlace != "right"|| s.TypeOfPlace != "left")
                .OrderBy(s => s.RowNumber).ThenBy(s => s.SeatNumber).ToList();

            var regularSeats = freeSeats
                .Where(s => s.TypeOfPlace == "left"|| s.TypeOfPlace != "right")
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
            var rows = seats.GroupBy(s => s.RowNumber);
            List<HallSeat> bestCandidate = null;
            double bestScore = double.MaxValue;

            int totalRows = seats.Max(s => s.RowNumber); // או להביא מבחוץ
           
            foreach (var row in rows)
            {
                var rowSeats = row.OrderBy(s => s.SeatNumber).ToList();
                if (rowSeats.Count < count) continue;

                // חישוב אמצע השורה
                double middle = (rowSeats.First().SeatNumber + rowSeats.Last().SeatNumber) / 2.0;

                for (int i = 0; i <= rowSeats.Count - count; i++)
                {
                    var candidate = rowSeats.Skip(i).Take(count).ToList();

                    // בדיקה שזה רצף אמיתי
                    if (candidate.Last().SeatNumber - candidate.First().SeatNumber != count - 1)
                        continue;

                    // חישוב מרכז הרצף
                    double candidateMiddle = (candidate.First().SeatNumber + candidate.Last().SeatNumber) / 2.0;

                    // כמה הוא רחוק מהאמצע
                    double distanceFromCenter = Math.Abs(candidateMiddle - middle);

                    // ניקוד (אפשר גם להוסיף משקל לשורה אם רוצים)
                    // 🎯 מרכז הרצף

                    // 🎯 מרכז השורה (אמיתי)
                    int realMin = rowSeats.Min(s => s.SeatNumber);
                    int realMax = rowSeats.Max(s => s.SeatNumber);
                    double rowMiddle = (realMin + realMax) / 2.0;

                    // 🎯 מרחק מהמרכז
                    double centerDistance = Math.Abs(candidateMiddle - rowMiddle);

                    // נרמול
                    double maxDistance = rowMiddle;
                    double normalizedCenter = centerDistance / maxDistance;

                    // 🎯 נרמול שורה (קדימה = טוב)
                    double normalizedRow = (double)row.Key / totalRows;

                    // 🎯 score — שורה יותר חשובה!
                    double score = normalizedRow * 0.7 + normalizedCenter * 0.3;

                    // שמירת הכי טוב
                    if (score < bestScore)
                    {
                        bestScore = score;
                        bestCandidate = candidate;
                    }
                }
            }

            return bestCandidate;
        }

        private List<HallSeat> FindBestBlock(List<HallSeat> seats, int count)
        {
            var rows = seats.GroupBy(s => s.RowNumber).OrderBy(g => g.Key).ToList();

            List<HallSeat> bestCandidate = null;
            double bestScore = double.MaxValue;

            int totalRows = rows.Count;

            // ⚠️ מרכז קבוע של שורה (לא לפי מושבים פנויים!)
            int minSeatNumber = 1;
            int maxSeatNumber = 50;
            double rowMiddle = (minSeatNumber + maxSeatNumber) / 2.0;

            for (int i = 0; i < rows.Count - 1; i++)
            {
                var row1 = rows[i].OrderBy(s => s.SeatNumber).ToList();
                var row2 = rows[i + 1].OrderBy(s => s.SeatNumber).ToList();

                int half1 = count / 2;
                int half2 = count - half1;

                var candidates1 = GetSequentialBlocks(row1, half1);
                var candidates2 = GetSequentialBlocks(row2, half2);

                foreach (var c1 in candidates1)
                {
                    foreach (var c2 in candidates2)
                    {

                        var combined = c1.Concat(c2).ToList();

                        // 🎯 מרכז של כל חלק
                        double midC1 = (c1.First().SeatNumber + c1.Last().SeatNumber) / 2.0;
                        double midC2 = (c2.First().SeatNumber + c2.Last().SeatNumber) / 2.0;

                        // 🎯 מרכז כולל
                        double combinedMiddle = (midC1 + midC2) / 2.0;

                        // 🎯 מרחק מהמרכז האמיתי של השורה
                        double centerDistance = Math.Abs(combinedMiddle - rowMiddle);

                        // 🔹 נרמול מרחק מהמרכז
                        double maxDistance = rowMiddle;
                        double normalizedCenter = centerDistance / maxDistance;

                        // 🔹 נרמול מיקום שורה (קדימה = טוב)
                        double normalizedRow = (double)rows[i].Key / totalRows;
                        var score = 0.0;
                        if (normalizedCenter < 0.2)
                        {
                            score = normalizedRow; // רק שורה קובעת
                        }
                        else
                        {
                            score = normalizedRow * 0.7 + normalizedCenter * 0.3;
                        }
                        if (score < bestScore)
                        {
                            bestScore = score;
                            bestCandidate = combined;
                        }
                    }
                }
            }

            return bestCandidate;
        }
        private List<List<HallSeat>> GetSequentialBlocks(List<HallSeat> seats, int count)
        {
            var result = new List<List<HallSeat>>();

            for (int i = 0; i <= seats.Count - count; i++)
            {
                var candidate = seats.Skip(i).Take(count).ToList();

                if (candidate.Last().SeatNumber - candidate.First().SeatNumber == count - 1)
                {
                    result.Add(candidate);
                }
            }

            return result;
        }
    }
}