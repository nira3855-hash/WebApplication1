using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service.Dto;
using Service.Services;
using Service.Interface;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderDetailController : ControllerBase
    {
        private readonly OrderDetailIService _orders;

        public OrderDetailController(OrderDetailIService orders)
        {
            _orders = orders;
        }

        // GET: api/OrderDetail
        [HttpGet]
        [Authorize(Roles = "0,1")]
        public async Task<ActionResult<List<OrderDetailDto>>> Get()
        {
            var list = await _orders.GetAllAsync();
            return Ok(list);
        }

        // GET api/OrderDetail/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetByUser(int id)
        {
            try
            {
                var order = await _orders.GetByIdAsync(id);
                return Ok(order);
            }
            catch
            {
                return NotFound(new
                {
                    ErrorCode = 404,
                    Message = $"Order with ID {id} was not found."
                });
            }
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> Get(int userId)
        {
            try
            {
                var order = await _orders.GetByUserIdAsync(userId);
                return Ok(order);
            }
            catch
            {
                return NotFound(new
                {
                    ErrorCode = 404,
                    Message = $"Order with ID {userId} was not found."
                });
            }
        }
        [HttpPost("add-to-cart")]
        public async Task<IActionResult> AddToCart(OrderDetailCreateDto dto)
        {
            await _orders.AddToCartItemAsync(dto);
            return Ok();
        }

        [HttpPost("complete")]
        public async Task<IActionResult> CompleteOrder(OrderDetailCreateDto dto)
        {
            await _orders.CompleteOrderItemAsync(dto);
            return Ok();
        }

        // PUT api/OrderDetail/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] OrderDetailDto value)
        {
            try
            {
                await _orders.UpdateItemAsync(id, value);
                return Ok();
            }
            catch
            {
                return NotFound(new
                {
                    ErrorCode = 404,
                    Message = $"Order with ID {id} was not found."
                });
            }
        }

        // DELETE api/OrderDetail/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _orders.DeleteItemAsync(id);
                return Ok();
            }
            catch
            {
                return NotFound(new
                {
                    ErrorCode = 404,
                    Message = $"Order with ID {id} was not found."
                });
            }
        }

    }
}