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
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<List<OrderDetailDto>>> Get()
        {
            var list = await _orders.GetAllAsync();
            return Ok(list);
        }

        // GET api/OrderDetail/5
        [HttpGet("{id}")]
        [Authorize(Roles = "0,1")]
        public async Task<IActionResult> Get(int id)
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
        [Authorize(Roles = "0,1")]
        public async Task<IActionResult> GetByUser(int userId)
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
        [HttpGet("event/{eventId}")]
        [Authorize(Roles = "0,1")]
        public async Task<IActionResult> GetByEvent(int eventId)
        {
            try
            {
                var order = await _orders.GetOrdersByEventIdAsync(eventId);
                return Ok(order);
            }
            catch
            {
                return NotFound(new
                {
                    ErrorCode = 404,
                    Message = $"Order with ID {eventId} was not found."
                });
            }
        }
        [HttpPost("add-to-cart")]
        [Authorize(Roles = "0,1")]
        public async Task<IActionResult> AddToCart(OrderDetailCreateDto dto)
        {
            await _orders.AddToCartItemAsync(dto);
            return Ok();
        }

        [HttpPost("complete")]
        [Authorize(Roles = "0,1")]
        public async Task<IActionResult> CompleteOrder(OrderDetailCreateDto dto)
        {
            await _orders.CompleteOrderItemAsync(dto);
            return Ok();
        }

        // PUT api/OrderDetail/5
        [HttpPut("{id}")]
        [Authorize(Roles = "0,1")]
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
        [Authorize(Roles = "0,1")]
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
        [HttpPost("CompleteOrderSeats")]
        public async Task<IActionResult> CompleteMultipleOrder([FromBody] CompleteMultipleSeatsDto dto)
        {
            try
            {
                await _orders.CompleteMultipleOrderAsync(dto);
                return Ok("Seats booked successfully");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost("AddToCartSeats")]
        public async Task<IActionResult> AddMultipleToCart([FromBody] CompleteMultipleSeatsDto dto)
        {
            try
            {
                await _orders.AddMultipleToCartAsync(dto);
                return Ok("Seats booked successfully");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}