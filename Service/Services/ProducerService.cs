using AutoMapper;
using Repository.Entities;
using Repository.Interfaces;
using Service.Dto;
using Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Service.Services
{
    public class ProducerService : IService<ProducerDto>
    {
        private readonly IRepository<User> users;
        private readonly IRepository<Producer> repository;
        private readonly IMapper mapper;

        public ProducerService(IRepository<Producer> repository, IRepository<User> users, IMapper mapper)
        {
            this.repository = repository;
            this.mapper = mapper;
            this.users = users;
        }

        public async Task<ProducerDto> AddItemAsync(ProducerDto dto)
        {
            // האם המשתמש קיים
            var userEntity = await users.GetByIdAsync(dto.UserId);
            if (userEntity == null) throw new Exception("User not found, register as user first");

            // האם הוא כבר מפיק
            var existingProducer = await repository.GetByIdAsync(dto.UserId);
            if (existingProducer != null) throw new Exception("User is already a producer");

            if (dto.CompanyName == null)
                throw new Exception("CompanyName is must field");

            // עדכון ROLE
            userEntity.UserRole = 1; // 1 Producer מייצג
            await users.UpdateItemAsync(userEntity.Id, userEntity);

            // יצירת האובייקט
            var newProducer = new Producer
            {
                UserId = dto.UserId,
                CompanyName = dto.CompanyName,
                Bio = dto.Bio
                // מקשר אוטומטי לUSER בUSERS
            };

            // הוספה
            var savedProducer = await repository.AddItemAsync(newProducer);
            return mapper.Map<ProducerDto>(savedProducer);
        }

        public bool IsValidEmail(string email)
        {
            string pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            return Regex.IsMatch(email, pattern);
        }

        public async Task DeleteItemAsync(int id)
        {
            var producer = await repository.GetByIdAsync(id);
            if (producer == null)
                throw new Exception("Producer not found");

            // מחזירים את המשתמש להיות רגיל
            var user = await users.GetByIdAsync(producer.UserId);
            if (user != null)
            {
                user.UserRole = 0;
                await users.UpdateItemAsync(user.Id, user);
            }

            await repository.DeleteItemAsync(id);
        }

        public async Task<List<ProducerDto>> GetAllAsync()
        {
            var list = await repository.GetAllAsync();
            return mapper.Map<List<Producer>, List<ProducerDto>>(list);
        }

        public async Task<ProducerDto> GetByIdAsync(int id)
        {
            var producer = await repository.GetByIdAsync(id);
            if (producer == null)
                throw new Exception("User not exist");

            return mapper.Map<Producer, ProducerDto>(producer);
        }

        public async Task UpdateItemAsync(int id, ProducerDto dto)
        {
            // קבלת העצם
            var producer = await repository.GetByIdAsync(id);
            if (producer == null)
                throw new Exception("Producer not found");

            // עדכון פרטי המשתמש אם נשלח 
            if (dto.User != null)
            {
                var userEntity = await users.GetByIdAsync(producer.UserId);
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
                    var existingUser = (await users.GetAllAsync())
                        .FirstOrDefault(u => u.email == dto.User.email && u.Id != userEntity.Id);

                    if (existingUser != null)
                        throw new Exception("Email already exists");

                    userEntity.email = dto.User.email;
                }

                // שמירת המשתמש המעודכן
                await users.UpdateItemAsync(userEntity.Id, userEntity);
            }

            // עדכון שדות Producer עצמו
            if (!string.IsNullOrEmpty(dto.CompanyName))
                producer.CompanyName = dto.CompanyName;

            if (dto.Bio != null)
                producer.Bio = dto.Bio;

            // עדכון
            await repository.UpdateItemAsync(id, producer);
        }
    }
}