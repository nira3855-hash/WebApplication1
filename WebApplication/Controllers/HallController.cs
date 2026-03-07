using Microsoft.AspNetCore.Mvc;
using Repository.Entities;
using Service.Dto;
using Service.Interface;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HallController : ControllerBase
    {
        private readonly IService<HallDto> halls;
        public HallController(IService<HallDto> hall)
        {
            this.halls = hall;
        }
        // GET: api/<HallController>
        [HttpGet]
        public List<HallDto> Get()
        {
            return halls.GetAll();
        }

        // GET api/<HallController>/5
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            try
            {
                var res = halls.GetById(id);
                return Ok(res);
            }
            catch
            {
                return NotFound(new
                {
                    ErrorCode = 404,
                    Message = $"Hall with ID {id} was not found."
                });
            }
        }

        // POST api/<HallController>
        [HttpPost]
        public HallDto Post([FromBody] HallDto value)
        {
            return halls.AddItem(value);
        }

        // PUT api/<HallController>/5
        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] HallDto value)
        {
            try
            {
                halls.UpdateItem(id, value);
            }
            catch (Exception ex)
            {
                return NotFound(new
                {
                    ErrorCode = 404,
                    Message = $"Hall with ID {id} was not found."
                });
            }

            // 3. רק אם נמצא ועודכן, מחזירים Ok
            return Ok();
        }

        // DELETE api/<HallController>/5
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            try
            {
                halls.DeleteItem(id);
            }
            catch (Exception ex)
            {
                return NotFound(new
                {
                    ErrorCode = 404,
                });
            }
            return Ok();
        }
    }
}
