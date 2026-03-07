using Microsoft.AspNetCore.Mvc;
using Repository.Entities;
using Service.Dto;
using Service.Interface;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HallSeatController : ControllerBase
    {
        private readonly IService<HallSeatDto> hallSeat;
        public HallSeatController(IService<HallSeatDto> hallSeat)
        {
            this.hallSeat = hallSeat;
        }
        // GET: api/<HallSeatController>
        [HttpGet]
        public List<HallSeatDto> Get()
        {
            return hallSeat.GetAll();
        }

        // GET api/<HallSeatController>/5
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            try
            {
                var res = hallSeat.GetById(id);
                return Ok(res);
            }
            catch
            {
                return NotFound(new
                {
                    ErrorCode = 404,
                    Message = $"HallSeat with ID {id} was not found."
                });
            }
        }

        // POST api/<HallSeatController>
        [HttpPost]
        public HallSeatDto Post([FromBody] HallSeatDto value)
        {
            return hallSeat.AddItem(value);
        }

        // PUT api/<HallSeatController>/5
        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] HallSeatDto value)
        {
            try
            {
                hallSeat.UpdateItem(id, value);
            }
            catch (Exception ex)
            {
                return NotFound(new
                {
                    ErrorCode = 404,
                    Message = $"HallSeat with ID {id} was not found."
                });
            }

            // 3. רק אם נמצא ועודכן, מחזירים Ok
            return Ok();
        }

        // DELETE api/<HallSeatController>/5
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            try
            {
                hallSeat.DeleteItem(id);
            }
            catch (Exception ex)
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
