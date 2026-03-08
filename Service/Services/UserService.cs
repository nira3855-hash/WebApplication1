using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Repository.Entities;
using Repository.Interfaces;
using Service.Dto;
using Service.Interface;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Service.Services
{
    public class UserService : UserIService
    {
        private readonly IRepository<User> repository;
        private readonly IMapper mapper;
        private readonly IConfiguration _config;
        public UserService(IRepository<User> repository, IMapper mapper, IConfiguration _config)
        {
            this.repository = repository;
            this.mapper = mapper;
            this._config = _config;
        }
        public UserDto AddItem(UserRegisterDto item)
        {
            item.password = BCrypt.Net.BCrypt.HashPassword(item.password);
            
            var userEntity = mapper.Map<User>(item);

            userEntity.UserRole = 0;
   
            var savedUser = repository.AddItem(userEntity);
            return mapper.Map<UserDto>(savedUser);
            
        }
        private string GenerateJwtToken(User user)
        {
            // 1. הגדרת ה-Key וה-Credentials (חתימה)
            // המפתח חייב להיות לפחות 32 תווים כדי ש-HmacSha256 יעבוד
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            // 2. הגדרת ה-Claims
            // אלו הפרטים שיהיו מוצפנים בתוך הטוקן ונוכל לשלוף אותם בכל בקשה
            var claims = new[]
            {
        new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
        new Claim(JwtRegisteredClaimNames.Email, user.email),
        new Claim(ClaimTypes.Name, user.Name),
        new Claim(ClaimTypes.Role, user.UserRole.ToString()), // חשוב להרשאות!
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()) // מזהה ייחודי לטוקן
    };

            // 3. יצירת אובייקט הטוקן
            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(1), // תוקף הטוקן (למשל 1 שעות)
                signingCredentials: credentials);

            // 4. המרת אובייקט הטוקן למחרוזת (String)
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    
        
        public string Login(UserLogin item)
        {
            // 1. שליפת המשתמש מה-Repository (באמצעות ה-Email)
            var user = repository.GetAll().FirstOrDefault(u => u.email == item.email);

            // 2. אימות הסיסמה בעזרת BCrypt
            // אנחנו משווים את סיסמת הטקסט הנקי מה-DTO מול ה-Hash ששמור ב-DB
            if (user == null || !BCrypt.Net.BCrypt.Verify(item.password, user.password))
            {
                return null; // או לזרוק Exception מסוג Unauthorized
            }

            // 3. יצירת הטוקן (שימוש בפונקציה שכתבנו קודם)
            string token = GenerateJwtToken(user);

            return token;
        }
        
        public void DeleteItem(int id)
        {
            var user = repository.GetById(id);

            if (user == null)
                throw new NotImplementedException();
            repository.DeleteItem(id);
        }

        public List<UserDto> GetAll()
        {
             
            return mapper.Map<List<User>, List<UserDto>>(repository.GetAll());
        }

        public UserDto GetById(int id)
        {
            var user = repository.GetById(id);
            if(user==null)
                throw new NotImplementedException();
            return mapper.Map<User, UserDto>(user);
        }

        public void UpdateItem(int id, UserDto dto)
        {
            var user = repository.GetById(id);
            if (user == null)
                throw new NotImplementedException();
            
            user.Name = dto.Name;
            user.email = dto.email;
            
            repository.UpdateItem(id, user);

        }
    }
}
