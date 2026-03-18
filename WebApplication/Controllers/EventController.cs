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
        [Authorize(Roles = "1")]
        public async Task<IActionResult> Put(int id, [FromForm] EventDto value)
        {
            // 1. בדיקה אם הועלה קובץ חדש
            if (value.FileImage != null)
            {
                // יצירת שם ייחודי לקובץ כדי למנוע דריסת קבצים בשם זהה
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(value.FileImage.FileName);

                // נתיב לתיקיית images בתוך wwwroot
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", fileName);

                // שמירת הקובץ פיזית בשרת
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await value.FileImage.CopyToAsync(stream);
                }

                // עדכון ה-URL שיישמר במסד הנתונים
                value.ImageUrl = "/images/" + fileName;
            }

            try
            {
                // 2. שליחה לעדכון ב-Service
                await events.UpdateEventAsync(id, value);

                // מחזירים את האובייקט המעודכן כדי שה-Frontend יוכל להציג אותו מיד
                return Ok(value);
            }
            catch (Exception ex)
            {
                return NotFound(new { Message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "1")] // מפיק
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await events.DeleteEventAsync(id);
                return Ok(new { Message = "האירוע נמחק בהצלחה." });
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { Message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                // מחזירים 400 כדי שה-Frontend יבין שזו חסימה לוגית
                return BadRequest(new { Message = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(500, "שגיאה פנימית בשרת.");
            }
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