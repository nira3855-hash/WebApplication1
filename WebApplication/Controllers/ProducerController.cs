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
    public class ProducerController : ControllerBase
    {
        private readonly IService<ProducerDto> _producers;

        public ProducerController(IService<ProducerDto> producers)
        {
            _producers = producers;
        }

        // GET: api/Producer
        [HttpGet]
        public async Task<ActionResult<List<ProducerDto>>> Get()
        {
            // מחזיר את כל המפיקים
            var list = await _producers.GetAllAsync();
            return Ok(list);
        }

        // GET: api/Producer/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                var producer = await _producers.GetByIdAsync(id);
                return Ok(producer);
            }
            catch (Exception)
            {
                return NotFound(new
                {
                    ErrorCode = 404,
                    Message = $"Producer with ID {id} was not found."
                });
            }
        }

        // POST: api/Producer
       
        [HttpPost]
        [Authorize(Roles = "0")]
        public async Task<ProducerDto> Create([FromBody] ProducerDto value)
        {
            // יצירת מפיק חדש
            return await _producers.AddItemAsync(value);
        }

        // PUT: api/Producer/5
        [HttpPut("{id}")]
        [Authorize(Roles = "0,1")]
        public async Task<IActionResult> Put(int id, [FromBody] ProducerDto value)
        {
            try
            {
                // עדכון מפיק קיים
                await _producers.UpdateItemAsync(id, value);
                return Ok(); // רק אם נמצא ועודכן, מחזירים Ok
            }
            catch (Exception)
            {
                return NotFound(new
                {
                    ErrorCode = 404,
                    Message = $"Producer with ID {id} was not found."
                });
            }
        }

        // DELETE: api/Producer/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "0,1")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                // מחיקה של מפיק
                await _producers.DeleteItemAsync(id);
                return Ok();
            }
            catch (Exception)
            {
                return NotFound(new
                {
                    ErrorCode = 404,
                    Message = $"Producer with ID {id} was not found."
                });
            }
        }
    }
}