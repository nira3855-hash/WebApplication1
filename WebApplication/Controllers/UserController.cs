using System.IO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Repository.Entities;
using Repository.Interfaces;
using Service.Interface;
using Service.Dto;
using Microsoft.AspNetCore.Http.HttpResults;
using Service.Services;
using Microsoft.AspNetCore.Authorization;
namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserIService users;
        

        public UserController(UserIService user)
        {
            this.users = user;
        }
        // GET: CustomerController
        [HttpGet]
        
        public List<UserDto> Get()
        {
            return users.GetAll();

        }

       

        [HttpPost("login")]
         public IActionResult Login([FromBody] UserLogin loginDto)
         {
             // 1. קריאה ל-Service לביצוע האימות וקבלת הטוקן
             var token = users.Login(loginDto);

             // 2. בדיקה האם האימות נכשל
             if (string.IsNullOrEmpty(token))
             {
                 // מחזירים 401 Unauthorized אם הפרטים שגויים
                 return Unauthorized(new { message = "אימייל או סיסמה שגויים" });
             }

             // 3. החזרת הטוקן בתוך אובייקט JSON
             // הלקוח (Frontend) יקבל: { "token": "eyJhbG..." }
             return Ok(new { token = token });
         }
        
        // GET: CustomerController/Details/5
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            
            try
            {
                var res=users.GetById(id);
                return Ok(res);
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



        // POST api/<CustomerController>/5
        [HttpPost]
        public UserDto Create([FromBody] UserRegisterDto value)//האם זה לא צריך להיות fromBody?וגם למה זה create ולא post
        {
            return users.AddItem(value);

        }


        // PUT api/<CustomerController>/5
        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] UserDto value)
        {
            try
            {
               users.UpdateItem(id, value);
            }
            catch (Exception ex)
            {
                return NotFound(new
                {
                    ErrorCode = 404,
                    Message = $"User with ID {id} was not found."
                });
            }

            // 3. רק אם נמצא ועודכן, מחזירים Ok
            return Ok();
        }
        

        // DELETE api/<CustomerController>/5
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            try {
                users.DeleteItem(id);
            }
            catch(Exception ex)
            {
                return NotFound(new
                {
                    ErrorCode = 404,//למה אין massage פה? וכך בכול Controllers האחרים?
                });
            }
            return Ok();
        }

       
    }
}
