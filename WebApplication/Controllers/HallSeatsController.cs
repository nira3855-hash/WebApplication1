
using Microsoft.AspNetCore.Mvc;
using Service.Dto;
using Service.Interface;
using Service.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HallSeatsController : ControllerBase
    {
        private readonly IService<HallSeatDto> _hallSeatService;

        public HallSeatsController(IService<HallSeatDto> hallSeatService)
        {
            _hallSeatService = hallSeatService;
        }

        // GET: api/HallSeat
        [HttpGet]
        public async Task<ActionResult<List<HallSeatDto>>> Get()
        {
            var list = await _hallSeatService.GetAllAsync();
            return Ok(list);
        }

        // GET api/HallSeat/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                var item = await _hallSeatService.GetByIdAsync(id);
                return Ok(item);
            }
            catch (NotImplementedException)
            {
                return NotFound(new
                {
                    ErrorCode = 404,
                    Message = $"HallSeat with ID {id} was not found."
                });
            }
        }

        // POST api/HallSeat
        [HttpPost]
        public async Task<ActionResult<HallSeatDto>> Post([FromBody] HallSeatDto value)
        {
            var added = await _hallSeatService.AddItemAsync(value);
            return CreatedAtAction(nameof(Get), new { id = added.Id }, added);
        }

        // PUT api/HallSeat/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] HallSeatDto value)
        {
            try
            {
                await _hallSeatService.UpdateItemAsync(id, value);
                return Ok();
            }
            catch (NotImplementedException)
            {
                return NotFound(new
                {
                    ErrorCode = 404,
                    Message = $"HallSeat with ID {id} was not found."
                });
            }
        }

        // DELETE api/HallSeat/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _hallSeatService.DeleteItemAsync(id);
                return Ok();
            }
            catch (NotImplementedException)
            {
                return NotFound(new
                {
                    ErrorCode = 404,
                    Message = $"HallSeat with ID {id} was not found."
                });
            }
        }
    }
}

