using Microsoft.AspNetCore.Mvc;
using Repository.Entities;
using Service.Dto;
using Service.Interface;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProducerController : ControllerBase
    {
        private readonly IService<ProducerDto> Producers;
        public ProducerController(IService<ProducerDto> producers)
        {
            this.Producers = producers;
        }
        // GET: CustomerController
        [HttpGet]
        public List<ProducerDto> Get()
        {
            return Producers.GetAll();

        }


        // GET: CustomerController/Details/5
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            try
            {
                return Ok(Producers.GetById(id));
            }
            catch (Exception ex)
            {
               return NotFound(new
                {
                    ErrorCode = 404,
                    Message = $"Producer with ID {id} was not found."
                }); 
            }
        }



        // POST api/<CustomerController>/5
        [HttpPost]
        public ProducerDto Create(ProducerDto value)
        {
            return Producers.AddItem(value);

        }


        // PUT api/<CustomerController>/5
        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] ProducerDto value)
        {
            try
            {
                Producers.UpdateItem(id, value);
                return Ok();
            }
            catch (Exception ex)
            {
                return NotFound(new
                {
                    ErrorCode = 404,
                    Message = $"Producer with ID {id} was not found."
                });
            }
            
        }

        // DELETE api/<CustomerController>/5
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            try
            {
                Producers.DeleteItem(id);
                return Ok();
            }
            catch (Exception ex)
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
