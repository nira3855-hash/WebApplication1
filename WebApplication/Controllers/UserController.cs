using System.IO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Repository.Entities;
using Repository.Interfaces;
using Service.Interface;
using Service.Dto;
using Microsoft.AspNetCore.Http.HttpResults;
namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IService<UserDto> users;
        public UserController(IService<UserDto> user)
        {
            this.users = user;
        }
        // GET: CustomerController
        [HttpGet]
        public List<UserDto> Get()
        {
            return users.GetAll();

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
        public UserDto Create(UserDto value)//האם זה לא צריך להיות fromBody?וגם למה זה create ולא post
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
