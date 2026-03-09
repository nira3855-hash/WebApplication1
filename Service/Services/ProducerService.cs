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
using System.Text.RegularExpressions;
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
            // האם המשתמש קיים
            var userEntity = users.GetById(dto.UserId);
            if (userEntity == null) throw new Exception("User not found lets sign like user first");
            
            // האם הוא כבר מפיקrepository.GetAll() 
            var existingProducer = repository.GetById(dto.UserId);
            if (existingProducer != null) throw new Exception("User is already a producer");
            if(dto.CompanyName==null)
                throw new Exception("CompanyName is must field");

            // עדכון ROLE
            userEntity.UserRole = 1; // 1 Producer מייצג 
            users.UpdateItem(userEntity.Id, userEntity);
            //  יצירת האובייקט
            var newProducer = new Producer
            {
                UserId = dto.UserId, 
                CompanyName = dto.CompanyName,
                Bio = dto.Bio
                //מקשר אוטומטי לUSER בUSERS
            };

            //  הוספה
            var savedProducer = repository.AddItem(newProducer);
           
            return mapper.Map<ProducerDto>(savedProducer);
        }
        public bool IsValidEmail(string email)
        {
            string pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            return Regex.IsMatch(email, pattern);
        }
        public void DeleteItem(int id)
        {
            var producer = repository.GetById(id);
            if (producer == null)
                throw new Exception("Producer not found");

            // מחזירים את המשתמש להיות רגיל
            var user = users.GetById(producer.UserId);
            if (user != null)
            {
                user.UserRole = 0;
                users.UpdateItem(user.Id, user);
            }

            repository.DeleteItem(id);
        }

        public List<ProducerDto> GetAll()
        {
            return mapper.Map<List<Producer>, List<ProducerDto>>(repository.GetAll());
        }

        public ProducerDto GetById(int id)
        {
            var producer = repository.GetById(id);
            if (producer == null)
               throw new Exception("User not exist");

            return mapper.Map<Producer, ProducerDto>(producer);

        }

        public void UpdateItem(int id, ProducerDto dto)
        {
            // קבלת העצם
            var producer = repository.GetById(id);
            if (producer == null)
                throw new Exception("Producer not found");

            // עדכון פרטי המשתמש אם נשלח 
            if (dto.User != null)
            {
                var userEntity = users.GetById(producer.UserId);
                if (userEntity == null)
                    throw new Exception("Associated user not found");

                // עדכון שם רק אם נשלח ערך
                if (!string.IsNullOrEmpty(dto.User.Name))
                    userEntity.Name = dto.User.Name;

                // עדכון אימייל רק אם נשלח ערך
                if (!string.IsNullOrEmpty(dto.User.email))
                {
                    // בדיקת פורמט אימייל
                    if (!IsValidEmail(dto.User.email))
                        throw new Exception("Invalid email format");

                    // בדיקה אם הEMAIL קיים כבר
                    var existingUser = users.GetAll()
                        .FirstOrDefault(u => u.email == dto.User.email && u.Id != userEntity.Id);

                    if (existingUser != null)
                        throw new Exception("Email already exists");

                    userEntity.email = dto.User.email;
                }

               
               
                // שמירת המשתמש המעודכן
                users.UpdateItem(userEntity.Id, userEntity);
            }

            // עדכון שדות Producer עצמו
            if (!string.IsNullOrEmpty(dto.CompanyName))
                producer.CompanyName = dto.CompanyName;
           
            if (dto.Bio != null)
                producer.Bio = dto.Bio;
            

            // עדכון
            repository.UpdateItem(id, producer);
        }
    }
}
