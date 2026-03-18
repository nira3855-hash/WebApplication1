using Microsoft.AspNetCore.Authorization;
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
        //[Authorize(Roles = "0,1")]
        public async Task<List<EventDto>> Get()
        {
            // מחזיר את כל האירועים (אסינכרוני)
            return await events.GetAllEventsAsync();
        }

        // GET api/Event/5
        [HttpGet("{id}")]
        [Authorize(Roles = "0,1")]
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
        [Authorize(Roles = "1")]
        public async Task<EventDto> Post([FromForm] EventDto eventDto)
        {
            string imagePath = null;

            if (eventDto.FileImage != null)
            {
                // שימוש ב-Path.Combine נפרד לכל תיקייה כדי למנוע בעיות של לוכסנים
                string folder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");

                // יצירת התיקייה אם היא לא קיימת
                if (!Directory.Exists(folder))
                    Directory.CreateDirectory(folder);

                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(eventDto.FileImage.FileName);
                string fullPath = Path.Combine(folder, fileName);

                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    await eventDto.FileImage.CopyToAsync(stream);
                }

                // זה הנתיב שיישמר ב-DB: "/images/guid_name.jpg"
                imagePath = "/images/" + fileName;
            }

            eventDto.ImageUrl = imagePath;

            return await events.AddEventAsync(eventDto);
        }

        // PUT api/Event/5
        [HttpPut("{id}")]
        //[Authorize(Roles = "1")]
        public async Task<IActionResult> Put(int id, [FromForm] EventDto value)
        {
            // כאן כדאי להוסיף את אותה לוגיקת שמירת תמונה שיש ב-Post
            // כדי שאם המשתמש העלה תמונה חדשה בעריכה, היא תישמר ב-wwwroot
            if (value.FileImage != null)
            {
                // ... לוגיקת שמירת הקובץ (כמו ב-Post) ...
                // value.ImageUrl = "/images/" + fileName;
            }
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
        [Authorize(Roles = "1")]
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
        [Authorize(Roles = "0,1")]
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
        [Authorize(Roles = "0,1")]
        public async Task<List<Event>> Search([FromBody] string term)
        {
            // מחפש אירועים לפי מחרוזת חיפוש
            return await events.SearchEventsAsync(term);
        }

        // GET api/Event/location?loc=TelAviv
        [HttpGet("location")]
        public async Task<List<Event>> GetByLocation([FromBody] string loc)
        {
            // מחזיר אירועים לפי מיקום
            return await events.GetEventsByLocationAsync(loc);
        }
        // POST api/Event/getBestSeats
        [HttpPost("getBestSeats")]
        public async Task<List<HallSeatDto>> GetBestSeats([FromBody] BestSeatsForEvent dto )
        {
            //מחזיר מושבים טובים באירוע
            return await events.FindBestSeatsAsync(dto);
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