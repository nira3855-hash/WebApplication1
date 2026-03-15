
using Microsoft.AspNetCore.Authorization;
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
        private readonly HallSeatIService _hallSeatService;

        public HallSeatsController(HallSeatIService hallSeatService)
        {
            _hallSeatService = hallSeatService;
        }

        // GET: api/HallSeat
        [HttpGet]
        [Authorize(Roles = "0,1")]
        public async Task<ActionResult<List<HallSeatDto>>> Get()
        {
            var list = await _hallSeatService.GetAllAsync();
            return Ok(list);
        }

        // GET api/HallSeat/5
        [HttpGet("{id}")]
        [Authorize(Roles = "0,1")]
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
        [HttpGet("hall/{hallId}")]
        //[Authorize(Roles = "0,1")]
        public async Task<IActionResult> GetByHall(int hallId)
        {
            try
            {
                var item = await _hallSeatService.GetByHallIdAsync(hallId);
                return Ok(item);
            }
            catch (NotImplementedException)
            {
                return NotFound(new
                {
                    ErrorCode = 404,
                    Message = $"HallSeat with ID {hallId} was not found."
                });
            }
        }

        // POST api/HallSeat
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<HallSeatDto>> Post([FromBody] HallSeatDto value)
        {
            var added = await _hallSeatService.AddItemAsync(value);
            return CreatedAtAction(nameof(Get), new { id = added.Id }, added);
        }

        // PUT api/HallSeat/5
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
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
        [Authorize(Roles = "Admin")]
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

