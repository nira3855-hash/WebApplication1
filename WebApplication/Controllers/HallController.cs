using Microsoft.AspNetCore.Authorization;
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
    public class HallController : ControllerBase
    {
        private readonly IService<HallDto> halls;

        public HallController(IService<HallDto> hall)
        {
            this.halls = hall;
        }

        // GET: api/<HallController>
        [HttpGet]
        [Authorize(Roles = "1")]
        public async Task<List<HallDto>> Get()
        {
            return await halls.GetAllAsync(); // קריאה אסינכרונית
        }

        // GET api/<HallController>/5
        [HttpGet("{id}")]
        [Authorize(Roles = "1")]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                var res = await halls.GetByIdAsync(id); // קריאה אסינכרונית
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
        //[Authorize(Roles = "Admin")]
        public async Task<HallDto> Post([FromBody] HallDto value)
        {
            return await halls.AddItemAsync(value); // קריאה אסינכרונית
        }

        // PUT api/<HallController>/5
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Put(int id, [FromBody] HallDto value)
        {
            try
            {
                await halls.UpdateItemAsync(id, value); // קריאה אסינכרונית
            }
            catch (Exception)
            {
                return NotFound(new
                {
                    ErrorCode = 404,
                    Message = $"Hall with ID {id} was not found."
                });
            }

            // רק אם נמצא ועודכן, מחזירים Ok
            return Ok();
        }

        // DELETE api/<HallController>/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await halls.DeleteItemAsync(id); // קריאה אסינכרונית
            }
            catch (Exception)
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