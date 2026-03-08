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

        // עדכון משתמש - בדרך כלל נשתמש ב-DTO ייעודי לעדכון או ב-RegisterDto אם מעדכנים סיסמה
        void UpdateItem(int id, UserDto item);

        // שליפת משתמש לפי ID - מחזיר DTO נקי ללא סיסמה
        UserDto GetById(int id);

        // שליפת כל המשתמשים
        List<UserDto> GetAll();

        // מחיקת משתמש
        void DeleteItem(int id);
    }
}