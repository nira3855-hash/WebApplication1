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
        private readonly HallSeatIRepository seats;
        private readonly IMapper mapper;

        public HallService(IRepository<Hall> repository, HallSeatIRepository seats, IMapper mapper)
        {
            this.repository = repository;
            this.seats = seats;
            this.mapper = mapper;
        }
        //ולידציה ליצירת אולם
        private void ValidateHall(HallDto hall)
        {
            if (hall == null)
                throw new ArgumentNullException(nameof(hall), "האולם לא נשלח.");

            if (string.IsNullOrWhiteSpace(hall.name))
                throw new ArgumentException("שם האולם חובה.");

            if (string.IsNullOrWhiteSpace(hall.location))
                throw new ArgumentException("מיקום האולם חובה.");

            if (hall.numOfSeats <= 0)
                throw new ArgumentException("מספר המקומות חייב להיות גדול מ-0.");

            if (string.IsNullOrWhiteSpace(hall.shape))
                throw new ArgumentException("יש לבחור צורת אולם.");

            var validShapes = new[] { "Circle", "Auditorium", "Rectangle" };

            if (!validShapes.Contains(hall.shape))
                throw new ArgumentException("צורת האולם אינה תקינה.");
        }
        //יצירת מראה מקומות של אולם מלבן
        public List<HallSeat> GenerateRectangleHallSeatsWithPrice(HallDto hall)
        {
            List<HallSeat> seats = new List<HallSeat>();
            int rows = 10; // מספר שורות
            int seatsPerRow = hall.numOfSeats / rows;

            double baseAddPrice = 50; // מחיר מקסימום לקרבה לבמה
            double decrementPerRow = baseAddPrice / rows; // מחיר יורד בשורה הבאה

            int stageRow = 1; // הבמה תהיה בשורה 1

            for (int row = 1; row <= rows; row++)
            {
                for (int seat = 1; seat <= seatsPerRow; seat++)
                {
                    // חישוב תוספת מחיר לפי קרבה לבמה
                    double addPrice = Math.Max(0, baseAddPrice - decrementPerRow * (row - stageRow));

                    string typeOfPlace = row == stageRow ? "Stage" : "Normal";

                    seats.Add(new HallSeat
                    {
                        HallID = hall.Id,
                        RowNumber = row,
                        SeatNumber = seat,
                        AddPrice = addPrice,
                        TypeOfPlace = typeOfPlace
                    });
                }
            }
            return seats;
        }
        //צורת אודיטוריום
        public List<HallSeat>  GenerateAuditoriumHallSeats(HallDto hall)
        {
            List<HallSeat> seats = new List<HallSeat>();
            int totalSeats = hall.numOfSeats;
            int rows = 25;          // מספר שורות (טריבונה)
            int seatsPerRow = totalSeats / rows;

            int stageRow = 1;       // הבמה בחזית (שורה 1)
            double stagePrice = 100;
            double normalPrice = 0;

            for (int row = 1; row <= rows; row++)
            {
                // אפשר לעשות מעט עקומה כך שהכיסאות במרכז יותר קרובים
                int offset = (rows - row) / 2; // למרכז מעט curve

                for (int seat = 1; seat <= seatsPerRow; seat++)
                {
                    string typeOfPlace = row == stageRow ? "Stage" : "Normal";
                    double addPrice = typeOfPlace == "Stage" ? stagePrice : normalPrice;

                    seats.Add(new HallSeat
                    {
                        HallID = hall.Id ,
                        RowNumber = row,
                        SeatNumber = seat,
                        AddPrice = addPrice,
                        TypeOfPlace = typeOfPlace
                    });
                }
            }
            return seats;
        }
        //יצירת מקומות באולם בצורה עגולה עם במה באמצע
        private List<HallSeat> GenerateCircularHallSeatsWithStagePrice(HallDto hall)
        {
            List<HallSeat> seats = new List<HallSeat>();
            int rows = 10; // מספר שורות
            int seatsPerRow = hall.numOfSeats / rows;

            double stagePrice = 50;     // תוספת מחיר לכיסאות של הבמה
            double normalPrice = 0;      // תוספת מחיר לכיסאות רגילים

            int stageRow = 1; // השורה הראשונה היא הבמה

            for (int row = 1; row <= rows; row++)
            {
                for (int seat = 1; seat <= seatsPerRow; seat++)
                {
                    // סוג המקום
                    string typeOfPlace = row == stageRow ? "Stage" : "Normal";

                    // חישוב תוספת מחיר לפי סוג המקום
                    double addPrice = typeOfPlace == "Stage" ? stagePrice : normalPrice;

                    seats.Add(new HallSeat
                    {
                        HallID = hall.Id ,
                        RowNumber = row,
                        SeatNumber = seat,
                        AddPrice = addPrice,
                        TypeOfPlace = typeOfPlace
                    });
                   
                }
               
            }
            return seats;
        }
        public async Task<HallDto> AddItemAsync(HallDto item)
        {
            ValidateHall(item);
            List<HallSeat> seats = new List<HallSeat>();
            var entity = mapper.Map<HallDto, Hall>(item);
            var added = await repository.AddItemAsync(entity);
            item.Id = added.Id;
            switch (item.shape)
            {
                case "Circle":
                    seats=GenerateCircularHallSeatsWithStagePrice(item);
                    break;

                case "Auditorium":
                    seats= GenerateAuditoriumHallSeats(item);
                    break;

                default:
                    seats=GenerateRectangleHallSeatsWithPrice(item); // למשל ריבוע/Rectangle
                    break;
            }
            this.seats.AddRangeAsync(seats);
            return mapper.Map<Hall, HallDto>(added);
        }

        public async Task DeleteItemAsync(int id)
        {
            var hall = await repository.GetByIdAsync(id);
            if (hall == null)
                throw new ArgumentException("האולם לא קיים.");

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
                throw new ArgumentException("האולם לא קיים.");

            return mapper.Map<Hall, HallDto>(hall);
        }

        public async Task UpdateItemAsync(int id, HallDto item)
        {
            ValidateHall(item);
            var hall = await repository.GetByIdAsync(id);
            if (hall == null)
                throw new ArgumentException("האולם לא קיים.");

            hall.name = item.name;
            hall.location = item.location;
            hall.numOfSeats = item.numOfSeats;
            hall.shape = item.shape;

            await repository.UpdateItemAsync(id, hall);
        }
    }
}