using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Service.Dto;

namespace Service.Interface
{
    public interface UserIService
    {
        // פעולת הרישום - מקבלת DTO עם סיסמה ומחזירה DTO נקי
        Task<AuthResponseDto> AddItemAsync(UserRegisterDto item);

        // פעולת התחברות - מקבלת פרטי זיהוי ומחזירה טוקן או אובייקט משתמש
        Task<string> LoginAsync(UserLogin item);

        Task UpdateItemAsync(int id, UserDto item);

        Task<UserDto> GetByIdAsync(int id);

        Task<List<UserDto>> GetAllAsync();

        Task DeleteItemAsync(int id);
    }
}