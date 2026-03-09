using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Service.Dto;

namespace Service.Interface
{
    public interface UserIService
    {
        // פעולת הרישום - מקבלת DTO עם סיסמה ומחזירה DTO נקי
        UserDto AddItem(UserRegisterDto item);
        // פעולת התחברות - מקבלת פרטי זיהוי ומחזירה טוקן או אובייקט משתמש
        string Login(UserLogin item);
        void UpdateItem(int id, UserDto item);
        UserDto GetById(int id);
        List<UserDto> GetAll();
        void DeleteItem(int id);
    }
}