using Microsoft.AspNetCore.Mvc;
using Repository.Entities;
using Service.Dto;
using Service.Interface;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventController : ControllerBase
    {
        private readonly IService<EventDto> events;
        public EventController(IService<EventDto> even)
        {
            this.events = even;
        }
        // GET: api/<EventController>
        [HttpGet]
        public List<EventDto> Get()
        {
            return events.GetAll();
        }

        // GET api/<EventController>/5
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            try
            {
                var res = events.GetById(id);
                return Ok(res);
            }
            catch
            {
                return NotFound(new
                {
                    ErrorCode = 404,
                    Message = $"Event with ID {id} was not found."
                });
            }
        }

        // POST api/<EventController>
        [HttpPost]
        public EventDto Post([FromBody] EventDto value)
        {
            return events.AddItem(value);
        }

        // PUT api/<EventController>/5
        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] EventDto value)
        {
            try
            {
                events.UpdateItem(id, value);
            }
            catch (Exception ex)
            {
                return NotFound(new
                {
                    ErrorCode = 404,
                    Message = $"Event with ID {id} was not found."
                });
            }

            // 3. רק אם נמצא ועודכן, מחזירים Ok
            return Ok();
        }

        // DELETE api/<EventController>/5
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            try
            {
                events.DeleteItem(id);
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
