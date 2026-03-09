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
using System.Text.RegularExpressions;
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

        public bool IsValidPassword(string password)
        {
            string pattern = @"^(?=.*[A-Za-z])(?=.*\d)(?=.*[@$!%*?&]).{8,}$";
            return Regex.IsMatch(password, pattern);
        }
        public bool IsValidEmail(string email)
      {
        string pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
        return Regex.IsMatch(email, pattern);
      }

    //OK
    public UserDto AddItem(UserRegisterDto item)
        {
            //בדיקת ערכי חובה
            if (string.IsNullOrEmpty(item.email))
                throw new Exception("Email is required");

            if (string.IsNullOrEmpty(item.password))
                throw new Exception("Password is required");
            //בדיקת EMAIL תקין
            if (!IsValidEmail(item.email))
                throw new Exception("Invalid email format");
            //בדיקת סיסמא  חזקה
            if (!IsValidPassword(item.password))
                throw new Exception("Password must contain letters, numbers and special character");
            //בדיקה אם שם משתמש קיים כבר
            var existingUser = repository.GetAll()
                     .FirstOrDefault(u => u.email == item.email);
            if (existingUser != null)
                  throw new Exception("Email already exists");
            //הצפנת סיסמא
            item.password = BCrypt.Net.BCrypt.HashPassword(item.password);
            //הכנסת UserRole
            var userEntity = mapper.Map<User>(item);
            userEntity.UserRole = 0;
            // הכנסת משתמש
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
    
        //OK
        public string Login(UserLogin item)
        {
            //בדיקת ערכי חובה
            if (string.IsNullOrEmpty(item.email))
                throw new Exception("Email is required");

            if (string.IsNullOrEmpty(item.password))
                throw new Exception("Password is required");
            //שליפת המשתמש מה-Repository (באמצעות ה-Email)
            var user = repository.GetAll().FirstOrDefault(u => u.email == item.email);

            // אימות הסיסמה  
            if (user == null || !BCrypt.Net.BCrypt.Verify(item.password, user.password))
            {
                throw new UnauthorizedAccessException("Invalid email or password");
            }

            //יצירת הטוקן 
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
        //לא חובה אפשר להוריד
        public List<UserDto> GetAll()
        {
            return mapper.Map<List<User>, List<UserDto>>(repository.GetAll());
        }
        //לא חובה אפשר להוריד
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
                throw new Exception("User not found");

            // עדכון רק אם השדה קיים ולא ריק
            if (!string.IsNullOrEmpty(dto.Name))
                user.Name = dto.Name;

            if (!string.IsNullOrEmpty(dto.email))
            {
                //בדיקת EMAIL תקין
                if (!IsValidEmail(dto.email))
                    throw new Exception("Invalid email format");
                //בדיקה אם שם משתמש קיים כבר
                var existingUser = repository.GetAll()
                         .FirstOrDefault(u => u.email == dto.email);
                if (existingUser != null)
                    throw new Exception("Email already exists");
                user.email = dto.email;
            }
            repository.UpdateItem(id, user);
        }
    }
}
