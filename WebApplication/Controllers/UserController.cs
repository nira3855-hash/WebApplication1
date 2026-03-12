using Microsoft.AspNetCore.Mvc;
using Service.Dto;
using Service.Interface;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserIService _users;

        public UserController(UserIService user)
        {
            _users = user;
        }

        // GET: api/User
        [HttpGet]
        public async Task<ActionResult<List<UserDto>>> Get()
        {
            // מחזיר את כל המשתמשים
            var list = await _users.GetAllAsync();
            return Ok(list);
        }

        // GET: api/User/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                var user = await _users.GetByIdAsync(id);
                return Ok(user);
            }
            catch
            {
                return NotFound(new
                {
                    ErrorCode = 404,
                    Message = $"User with ID {id} was not found."
                });
            }
        }

        // POST api/User/login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLogin loginDto)
        {
            // קריאה ל-Service לביצוע האימות וקבלת הטוקן
            var token = await _users.LoginAsync(loginDto);

            // בדיקה האם האימות נכשל
            if (string.IsNullOrEmpty(token))
            {
                // מחזירים 401 Unauthorized אם הפרטים שגויים
                return Unauthorized(new { message = "אימייל או סיסמה שגויים" });
            }

            // החזרת הטוקן בתוך אובייקט JSON
            return Ok(new { token = token });
        }

        // POST api/User
        [HttpPost]
        public async Task<AuthResponseDto> Create([FromBody] UserRegisterDto value)
        {
            // הוספת משתמש חדש
            // הערה: צריך FromBody כדי שה־DTO יקבל את המידע מה-BODY של הבקשה
            // הערה נוספת: השם Create הוא לא חובה, POST בעצם מיועד ליצירה
            return await _users.AddItemAsync(value);
        }

        // PUT api/User/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] UserDto value)
        {
            try
            {
                // עדכון משתמש קיים
                await _users.UpdateItemAsync(id, value);
            }
            catch (Exception)
            {
                return NotFound(new
                {
                    ErrorCode = 404,
                    Message = $"User with ID {id} was not found."
                });
            }

            // רק אם נמצא ועודכן מחזירים Ok
            return Ok();
        }

        // DELETE api/User/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                // מחיקת משתמש
                await _users.DeleteItemAsync(id);
            }
            catch (Exception)
            {
                return NotFound(new
                {
                    ErrorCode = 404,
                    Message = $"User with ID {id} was not found." // הוספתי הודעת שגיאה כמו בשאר ה-Controllers
                });
            }

            return Ok();
        }
    }
}