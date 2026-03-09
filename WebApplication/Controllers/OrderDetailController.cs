using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Repository.Entities;
using Service.Dto;
using Service.Interface;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderDetailController : ControllerBase
    {
        private readonly OrderDetailIService orders;
        public OrderDetailController(OrderDetailIService order)
        {
            this.orders = order;
        }
        // GET: api/<OrderDetailController>
        [HttpGet]
        [Authorize(Roles = "0,1")]
        public List<OrderDetailDto> Get()
        {
            return orders.GetAll();
        }

        // GET api/<OrderDetailController>/5
        [HttpGet("{id}")]
       
        public IActionResult Get(int id)
        {
            try
            {
                var res = orders.GetById(id);
                return Ok(res);
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
        [HttpPost("add-to-cart")]
        public IActionResult AddToCart(OrderDetailCreateDto dto)
        {
            orders.AddToCartItem(dto);
            return Ok();
        }

        [HttpPost("complete")]
        public IActionResult CompleteOrder(OrderDetailCreateDto dto)
        {
            orders.CompleteOrderItem(dto);
            return Ok();
        }
       
       

        // PUT api/<OrderDetailController>/5
        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] OrderDetailDto value)
        {
            try
            {
                orders.UpdateItem(id, value);
            }
            catch (Exception ex)
            {
                return NotFound(new
                {
                    ErrorCode = 404,
                    Message = $"Order with ID {id} was not found."
                });
            }

            // 3. רק אם נמצא ועודכן, מחזירים Ok
            return Ok();
        }

        // DELETE api/<OrderDetailController>/5
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            try
            {
                orders.DeleteItem(id);
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
