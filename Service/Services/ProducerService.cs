using AutoMapper;
using Repository.Entities;
using Repository.Interfaces;
using Repository.Repositories;
using Service.Dto;
using Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Services
{
    public class ProducerService: IService<ProducerDto>
    {
        private readonly IRepository<User> users;
        private readonly IRepository<Producer> repository;
        private readonly IMapper mapper;
        public ProducerService(IRepository<Producer> repository,IRepository<User> users, IMapper mapper)
        {
            this.repository = repository;
            this.mapper = mapper;
            this.users = users;
        }
        public ProducerDto AddItem(ProducerDto dto)
        {
            // 1. בדיקה: האם המשתמש קיים?
            var userEntity = users.GetById(dto.UserId);
            if (userEntity == null) throw new Exception("User not found");

            // 2. בדיקה: האם הוא כבר מפיק?
            var existingProducer = repository.GetById(dto.UserId);
            if (existingProducer != null) throw new Exception("User is already a producer");

            // --- השלב החדש: עדכון ה-Role של המשתמש ---
            userEntity.UserRole = 1; // נניח ש-1 מייצג Producer
            users.UpdateItem(userEntity.Id, userEntity);
            // 3. יצירת האובייקט - שימי לב לשינוי כאן
            var newProducer = new Producer
            {
                UserId = dto.UserId, // משתמשים ב-ID ישירות
                CompanyName = dto.CompanyName,
                Bio = dto.Bio
                // לא מוסיפים כאן את האובייקט User = userEntity!
                // EF ידע לבד לעשות את הקישור לפי ה-UserId
            };

            // 4. שמירה ב-Repository
            var savedProducer = repository.AddItem(newProducer);

            // 5. החזרת התוצאה (הסרתי את ה-Return הכפול שהיה בסוף)
            return mapper.Map<ProducerDto>(savedProducer);
        }

        public void DeleteItem(int id)
        {
            throw new NotImplementedException();
        }

        public List<ProducerDto> GetAll()
        {
            return mapper.Map<List<Producer>, List<ProducerDto>>(repository.GetAll());
        }

        public ProducerDto GetById(int id)
        {
            var producer = repository.GetById(id);
            if (producer == null)
                throw new NotImplementedException();
            return mapper.Map<Producer, ProducerDto>(producer);

        }

        public void UpdateItem(int id, ProducerDto dto)
        {
            var producer = repository.GetById(id);
            if (producer == null)
                throw new NotImplementedException();

            if (dto.User != null)
            {
                var userEntity = users.GetById(dto.UserId);
                if (userEntity != null)
                {
                    // עדכון שדות המשתמש
                    userEntity.Name = dto.User.Name;
                    userEntity.email = dto.User.email; 

                    users.UpdateItem(userEntity.Id, userEntity);
                }
            }

            producer.Bio = dto.Bio;
            producer.CompanyName = dto.CompanyName;
             repository.UpdateItem(id, producer);
        }
    }
}
