using Microsoft.AspNetCore.Mvc;
using Repository.Entities;
using Service.Dto;
using Service.Interface;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventController : ControllerBase
    {
        private readonly EventIService events;

        public EventController(EventIService even)
        {
            this.events = even;
        }

        // GET: api/Event
        [HttpGet]
        public async Task<List<EventDto>> Get()
        {
            // מחזיר את כל האירועים (אסינכרוני)
            return await events.GetAllEventsAsync();
        }

        // GET api/Event/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                // מחזיר אירוע לפי ID (אסינכרוני)
                var res = await events.GetEventByIdAsync(id);
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

        // POST api/Event
        [HttpPost]
        public async Task<EventDto> Post([FromBody] EventDto value)
        {
            // מוסיף אירוע חדש (אסינכרוני)
            return await events.AddEventAsync(value);
        }

        // PUT api/Event/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] EventDto value)
        {
            try
            {
                // מעדכן אירוע קיים (אסינכרוני)
                await events.UpdateEventAsync(id, value);
            }
            catch (Exception)
            {
                return NotFound(new
                {
                    ErrorCode = 404,
                    Message = $"Event with ID {id} was not found."
                });
            }

            // רק אם נמצא ועודכן, מחזירים Ok
            return Ok();
        }

        // DELETE api/Event/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                // מוחק אירוע לפי ID (אסינכרוני)
                await events.DeleteEventAsync(id);
            }
            catch (Exception)
            {
                return NotFound(new
                {
                    ErrorCode = 404,
                    Message = $"Event with ID {id} was not found."
                });
            }
            return Ok();
        }

        // GET api/Event/producer/5
        [HttpGet("producer/{producerId}")]
        public async Task<List<Event>> GetByProducer(int producerId)
        {
            // מחזיר את כל האירועים של מפיק מסוים
            return await events.GetEventsByProducerIdAsync(producerId);
        }

        // GET api/Event/date/2026-03-10
        [HttpGet("date/{date}")]
        public async Task<List<Event>> GetByDate(DateTime date)
        {
            // מחזיר את כל האירועים בתאריך מסוים
            return await events.GetEventsByDateAsync(date);
        }

        // GET api/Event/upcoming
        [HttpGet("upcoming")]
        public async Task<List<Event>> GetUpcoming()
        {
            // מחזיר את כל האירועים הקרבים
            return await events.GetUpcomingEventsAsync();
        }

        // GET api/Event/search?term=concert
        [HttpGet("search")]
        public async Task<List<Event>> Search([FromQuery] string term)
        {
            // מחפש אירועים לפי מחרוזת חיפוש
            return await events.SearchEventsAsync(term);
        }

        // GET api/Event/location?loc=TelAviv
        [HttpGet("location")]
        public async Task<List<Event>> GetByLocation([FromQuery] string loc)
        {
            // מחזיר אירועים לפי מיקום
            return await events.GetEventsByLocationAsync(loc);
        }

        // GET api/Event/hall/5
        [HttpGet("hall/{hallId}")]
        public async Task<List<Event>> GetByHall(int hallId)
        {
            // מחזיר אירועים לפי אולם
            return await events.GetEventsByHallIdAsync(hallId);
        }
    }
}