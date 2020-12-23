using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EducationOnlinePlatform;
using EducationOnlinePlatform.ViewModels;
using Microsoft.Extensions.Logging;
using EducationOnlinePlatform.Models;
using System.Net;
using Newtonsoft.Json;

namespace EducationOnlinePlatform.Controllers
{
    public class ScheduleEventsController : Controller
    {
        private readonly ApplicationContext _context;

        private readonly ILogger _logger;

        public ScheduleEventsController(ApplicationContext context, ILogger<ScheduleEventsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: ScheduleEvents
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            _logger.LogInformation("Processing request {0}", Request.Path);
            var applicationContext = _context.ScheduleEvents.Include(s => s.EducationSet);
            return Ok(JsonConvert.SerializeObject(await applicationContext.ToListAsync()));
        }
        [HttpGet("{Id}")]
        public async Task<IActionResult> GetEvent(Guid? id)
        {
            _logger.LogInformation("Processing request {0}", Request.Path);
            if (id == null)
            {
                return NotFound();
            }

            var scheduleEvent = await _context.ScheduleEvents
                .Include(s => s.EducationSet)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (scheduleEvent == null)
            {
                return NotFound();
            }

            return Ok(JsonConvert.SerializeObject(scheduleEvent));
        }

        [HttpGet("EducationSet/{EducationId}")]
        public async Task<IActionResult> GetEventByEdcucationSet(Guid? EducationId)
        {
            _logger.LogInformation("Processing request {0}", Request.Path);
            if (EducationId == null)
            {
                return NotFound();
            }

            var scheduleEvents = await (_context.ScheduleEvents
                .Include(s => s.EducationSet)
                .Where(m => m.EducationSetId == EducationId)).ToListAsync();
            if (scheduleEvents.Count == 0)
            {
                return NotFound();
            }

            return Ok(JsonConvert.SerializeObject(scheduleEvents));
        }

        // GET: ScheduleEvents/Details/5
        [HttpGet("EducationSet/{EducationSetId}")]
        public async Task<IActionResult> Details(Guid? EducationSetId)
        {
            _logger.LogInformation("Processing request {0}", Request.Path);
            if (EducationSetId == null)
            {
                return NotFound();
            }

            var scheduleEvent = await _context.ScheduleEvents
                .Include(s => s.EducationSet)
                .FirstOrDefaultAsync(m => m.EducationSetId == EducationSetId);
            if (scheduleEvent == null)
            {
                return NotFound();
            }

            return Ok(scheduleEvent);
        }
        // POST: ScheduleEvents/Create/5

        [HttpPost("Create")]
        public async Task<IActionResult> Create([FromBody] AddScheduleEventViewModel scheduleEvent)
        {
            _logger.LogInformation("Processing request {0}", Request.Path);
            if (ModelState.IsValid)
            {
                var events = getEventsInEducationSetInTime(scheduleEvent.DateTimeFrom, scheduleEvent.DateTimeTo, scheduleEvent.EducationSetId);
                if(events.Result == 0)
                {
                    BadRequest("Conflict Events");
                }
                _context.Add(new ScheduleEvent {Name = scheduleEvent.Name, DateTimeFrom = scheduleEvent.DateTimeFrom, DateTimeTo = scheduleEvent.DateTimeTo, Description = scheduleEvent.Description, SubjectId = scheduleEvent.SubjectId, EducationSetId = scheduleEvent.EducationSetId });
                await _context.SaveChangesAsync();
                return Ok(new Result { Status = HttpStatusCode.OK, Message = "Event was created" }.ToString());
            }
            return BadRequest(new Result { Status = HttpStatusCode.BadRequest, Message = "Event was created" }.ToString());
        }

        private async Task<int> getEventsInEducationSetInTime(DateTime dateTimeFrom, DateTime dateTimeTo, Guid EducationSetId)
        {
            return (await (from e in _context.ScheduleEvents
                        where e.EducationSetId == EducationSetId &&
                        ((e.DateTimeFrom <= dateTimeFrom && e.DateTimeTo >= dateTimeFrom) ||
                        (e.DateTimeFrom <= dateTimeTo && e.DateTimeTo >= dateTimeTo))
                        select new { id = e.Id }).ToListAsync()).Count();
        }

        // POST: ScheduleEvents/Edit/5
        [HttpPost("Edit/{id}")]
        public async Task<IActionResult> Edit(Guid id, [FromBody] ScheduleEventUpdate scheduleEventUpdate)
        {
            _logger.LogInformation("Processing request {0}", Request.Path);
            if (id == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var scheduleEvent = _context.ScheduleEvents.FirstOrDefault(se => se.Id == id);
                if(scheduleEventUpdate.Name != scheduleEvent.Name && scheduleEventUpdate.Name != null)
                {
                    scheduleEvent.Name = scheduleEventUpdate.Name;
                }
                if (scheduleEventUpdate.Description != scheduleEvent.Description && scheduleEventUpdate.Description != null)
                {
                    scheduleEvent.Description = scheduleEventUpdate.Description;
                }
                if (scheduleEventUpdate.DateTimeFrom != scheduleEvent.DateTimeFrom && scheduleEventUpdate.DateTimeTo != scheduleEvent.DateTimeTo)
                {
                    var events = getEventsInEducationSetInTime(scheduleEvent.DateTimeFrom, scheduleEvent.DateTimeTo, scheduleEvent.EducationSetId);
                    if (events.Result == 0)
                    {
                        BadRequest("Conflict Events");
                    }
                    scheduleEvent.DateTimeTo = scheduleEventUpdate.DateTimeTo;
                    scheduleEvent.DateTimeFrom = scheduleEventUpdate.DateTimeFrom;
                }
                if (scheduleEventUpdate.SubjectId != scheduleEvent.SubjectId)
                {
                    scheduleEvent.SubjectId = scheduleEventUpdate.SubjectId;
                }
                if (scheduleEventUpdate.EducationSetId != scheduleEvent.EducationSetId)
                {
                    scheduleEvent.EducationSetId = scheduleEventUpdate.EducationSetId;
                }
                _context.Update(scheduleEvent);
                await _context.SaveChangesAsync();
                return Ok("Success Update");
            }
            return BadRequest("Bad Request");
        }

        // DELETE: ScheduleEvents/Delete/5
        [HttpDelete("Delete/{id}")]
        public async Task<IActionResult> Delete(Guid? id)
        {
            _logger.LogInformation("Processing request {0}", Request.Path);
            if (id == null)
            {
                return NotFound();
            }

            var scheduleEvent = await _context.ScheduleEvents
                .FirstOrDefaultAsync(m => m.Id == id);
            if (scheduleEvent == null)
            {
                return NotFound();
            }
            _context.Remove(scheduleEvent);
            await _context.SaveChangesAsync();

            return Ok("Succsess Remove");
        }
    }
}
