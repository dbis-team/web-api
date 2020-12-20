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
        public async Task<IActionResult> Index()
        {
            _logger.LogInformation("Processing request {0}", Request.Path);
            var applicationContext = _context.ScheduleEvents.Include(s => s.EducationSet);
            return View(await applicationContext.ToListAsync());
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

            return Ok(scheduleEvent);
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

        // POST: ScheduleEvents/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromBody] AddScheduleEventViewModel scheduleEvent)
        {
            _logger.LogInformation("Processing request {0}", Request.Path);
            if (ModelState.IsValid)
            {
                
                _context.Add(new ScheduleEvent {Name = scheduleEvent.Name, DateTime = scheduleEvent.DateTime, Description = scheduleEvent.Description, EducationSetId = scheduleEvent.EducationSetId });
                await _context.SaveChangesAsync();
                return Ok("Save Confirmed");
            }
            return BadRequest("Bad Request");
        }
        // POST: ScheduleEvents/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost("Edit/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,Name,Description,dateTime,EducationSetId")] ScheduleEventUpdate scheduleEventUpdate)
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
                if (scheduleEventUpdate.DateTime != scheduleEvent.DateTime && scheduleEventUpdate.DateTime != new DateTime())
                {
                    scheduleEvent.DateTime = scheduleEventUpdate.DateTime;
                }
                if (scheduleEventUpdate.EducationSetId != scheduleEvent.EducationSetId && scheduleEventUpdate.EducationSetId != new Guid())
                {
                    scheduleEvent.EducationSetId = scheduleEventUpdate.EducationSetId;
                }
                _context.Update(scheduleEvent);
                 await _context.SaveChangesAsync();
                return Ok("Success Update");
            }
            return BadRequest("Bad Request");
        }

        // GET: ScheduleEvents/Delete/5
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
