using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Repository.Entities;
using Repository.Interfaces;
using Service.Dto;
using Service.Interface;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Collections.Generic;

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
       



    
       
            // יצירת רשימת השורות לפי התמונה (דוגמה לגוש שמאלי עליון - אולם ג')
            List<SeatRow> theaterMap = new List<SeatRow>
            {
            // שורה | התחלה | סוף | שם גוש | מחיר
            new SeatRow { RowNumber = 1,  StartSeat = 12, EndSeat = 36 },
            new SeatRow { RowNumber = 2,  StartSeat = 8, EndSeat = 41},
            new SeatRow { RowNumber = 3,  StartSeat = 7,  EndSeat = 42 },
            new SeatRow { RowNumber = 4,  StartSeat = 6,  EndSeat = 44 },
            new SeatRow { RowNumber = 5,  StartSeat = 6,  EndSeat = 45 },
            new SeatRow { RowNumber = 6,  StartSeat = 5,  EndSeat = 47 },
            new SeatRow { RowNumber = 7, StartSeat = 5,  EndSeat = 48},
            new SeatRow { RowNumber = 8,  StartSeat = 4, EndSeat = 48},
            new SeatRow { RowNumber = 9,  StartSeat = 4, EndSeat = 48},
            new SeatRow { RowNumber = 10,  StartSeat = 3,  EndSeat = 49 },
            new SeatRow { RowNumber = 11,  StartSeat = 3,  EndSeat = 49 },
            new SeatRow { RowNumber = 12,  StartSeat = 2,  EndSeat = 50 },
            new SeatRow { RowNumber = 13,  StartSeat = 1,  EndSeat = 50 },
            new SeatRow { RowNumber = 14, StartSeat = 1,  EndSeat = 50  },
             new SeatRow { RowNumber = 15, StartSeat = 1,  EndSeat = 50 },
            new SeatRow { RowNumber = 16,  StartSeat = 1,  EndSeat = 50 },
            new SeatRow { RowNumber = 17,  StartSeat = 1,  EndSeat = 50 },
            new SeatRow { RowNumber = 18,  StartSeat = 1,  EndSeat = 50 },
            new SeatRow { RowNumber = 19,  StartSeat = 1,  EndSeat = 50 },
            new SeatRow { RowNumber = 20, StartSeat = 1,  EndSeat = 50  },
             new SeatRow { RowNumber = 21, StartSeat = 1,  EndSeat = 50 },
            new SeatRow { RowNumber = 22,  StartSeat = 1,  EndSeat = 50 },
            new SeatRow { RowNumber = 23,  StartSeat = 2,  EndSeat = 49 },
            new SeatRow { RowNumber = 24,  StartSeat = 2,  EndSeat = 49 },
            new SeatRow { RowNumber = 25,  StartSeat = 3,  EndSeat = 48 },
            new SeatRow { RowNumber = 26, StartSeat = 3,  EndSeat = 48},
             new SeatRow { RowNumber = 27, StartSeat = 4,  EndSeat = 47},
            new SeatRow { RowNumber = 28,  StartSeat = 4,  EndSeat = 47},
            new SeatRow { RowNumber = 29,  StartSeat = 4,  EndSeat = 47},
            new SeatRow { RowNumber = 30,  StartSeat = 6,  EndSeat = 45 }

        };

        private List<HallSeat> GenerateAuditoriumHallSeats(HallDto hall)
        {
            var seatsList = new List<HallSeat>();

            // הגדרת המפה המדויקת לפי התמונה
            var theaterMap = new List<SeatRow>
    {
                   // שורה | התחלה | סוף | שם גוש | מחיר
            new SeatRow { RowNumber = 1,  StartSeat = 12, EndSeat = 36 },
            new SeatRow { RowNumber = 2,  StartSeat = 8, EndSeat = 41},
            new SeatRow { RowNumber = 3,  StartSeat = 7,  EndSeat = 42 },
            new SeatRow { RowNumber = 4,  StartSeat = 6,  EndSeat = 44 },
            new SeatRow { RowNumber = 5,  StartSeat = 6,  EndSeat = 45 },
            new SeatRow { RowNumber = 6,  StartSeat = 5,  EndSeat = 47 },
            new SeatRow { RowNumber = 7, StartSeat = 5,  EndSeat = 48},
            new SeatRow { RowNumber = 8,  StartSeat = 4, EndSeat = 48},
            new SeatRow { RowNumber = 9,  StartSeat = 4, EndSeat = 48},
            new SeatRow { RowNumber = 10,  StartSeat = 3,  EndSeat = 49 },
            new SeatRow { RowNumber = 11,  StartSeat = 3,  EndSeat = 49 },
            new SeatRow { RowNumber = 12,  StartSeat = 2,  EndSeat = 50 },
            new SeatRow { RowNumber = 13,  StartSeat = 1,  EndSeat = 50 },
            new SeatRow { RowNumber = 14, StartSeat = 1,  EndSeat = 50  },
             new SeatRow { RowNumber = 15, StartSeat = 1,  EndSeat = 50 },
            new SeatRow { RowNumber = 16,  StartSeat = 1,  EndSeat = 50 },
            new SeatRow { RowNumber = 17,  StartSeat = 1,  EndSeat = 50 },
            new SeatRow { RowNumber = 18,  StartSeat = 1,  EndSeat = 50 },
            new SeatRow { RowNumber = 19,  StartSeat = 1,  EndSeat = 50 },
            new SeatRow { RowNumber = 20, StartSeat = 1,  EndSeat = 50  },
             new SeatRow { RowNumber = 21, StartSeat = 1,  EndSeat = 50 },
            new SeatRow { RowNumber = 22,  StartSeat = 1,  EndSeat = 50 },
            new SeatRow { RowNumber = 23,  StartSeat = 2,  EndSeat = 49 },
            new SeatRow { RowNumber = 24,  StartSeat = 2,  EndSeat = 49 },
            new SeatRow { RowNumber = 25,  StartSeat = 3,  EndSeat = 48 },
            new SeatRow { RowNumber = 26, StartSeat = 3,  EndSeat = 48},
             new SeatRow { RowNumber = 27, StartSeat = 4,  EndSeat = 47},
            new SeatRow { RowNumber = 28,  StartSeat = 4,  EndSeat = 47},
            new SeatRow { RowNumber = 29,  StartSeat = 4,  EndSeat = 47},
            new SeatRow { RowNumber = 30,  StartSeat = 6,  EndSeat = 45 }
           };

            foreach (var row in theaterMap)
            {
                for (int s = row.StartSeat; s <= row.EndSeat; s++)
                {
                    string typePlace = "";
                    double addPrice = 0.0;
                        // חישוב תוספת מחיר לפי הלוגיקה שלך
                    if (row.RowNumber <= 13 && s <= 16)
                    {
                         typePlace = "Stage left";// דוגמה לתוספת מחיר לגוש השמאלי
                        addPrice = 50.0;
                    }
                    if (row.RowNumber <= 13 && s  <=30&& s > 16)
                    {
                        typePlace = "Stage middle";// דוגמה לתוספת מחיר לגוש האמצעי
                        addPrice = 70.0;
                    }
                    if (row.RowNumber <= 13&&s > 30)
                    {
                        typePlace = "Stage right";// דוגמה לתוספת מחיר לגוש הימני
                        addPrice = 50.0; 
                    }
                    if (row.RowNumber > 13 && s <= 16)
                    {
                        typePlace = "left";// דוגמה לתוספת מחיר לגוש הימני
                        addPrice = 0.0;
                    }
                    if (row.RowNumber > 13 && s <= 30 && s > 16)
                    {
                        typePlace = "middle";// דוגמה לתוספת מחיר לגוש האמצעי
                        addPrice = 30.0;

                    }
                    if (row.RowNumber > 13 && s > 30)
                    {
                        typePlace = "right"; // דוגמה לתוספת מחיר לגוש הימני
                    }
                    seatsList.Add(new HallSeat
                    {
                        HallID = hall.Id,
                        RowNumber = row.RowNumber,
                        SeatNumber = s,
                        AddPrice = (double)addPrice,
                        TypeOfPlace = typePlace
                    });
                    }
                }
            return seatsList;
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
                    //seats = GenerateCircularHallSeatsWithStagePrice(item);
                    break;

                case "Auditorium":
                    seats = GenerateAuditoriumHallSeats(item);
                    break;

                default:
                    //seats = GenerateRectangleHallSeatsWithPrice(item); // למשל ריבוע/Rectangle
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