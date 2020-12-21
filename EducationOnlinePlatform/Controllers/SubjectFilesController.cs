using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EducationOnlinePlatform;
using EducationOnlinePlatform.Models;
using Microsoft.AspNetCore.Cors;
using EducationOnlinePlatform.ViewModels;
using Microsoft.Extensions.Logging;

namespace EducationOnlinePlatform.Controllers
{
    [EnableCors("CorsApi")]
    [ApiController]
    [Route("Files")]
    public class SubjectFilesController : Controller
    {
        ApplicationContext _context;

        private readonly ILogger _logger;
        public SubjectFilesController(ApplicationContext context, ILogger<SubjectFilesController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: SubjectFiles
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            _logger.LogInformation("Processing request {0}", Request.Path);
            return Ok(await _context.Files.ToListAsync());
        }

        // GET: SubjectFiles/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var subjectFile = await _context.Files
                .FirstOrDefaultAsync(m => m.Id == id);
            if (subjectFile == null)
            {
                return NotFound();
            }

            return Ok(subjectFile);
        }
        [HttpGet("Subject/{Subjectid}")]
        public async Task<IActionResult> DetailsSubject(Guid? Subjectid)
        {
            if (Subjectid == null)
            {
                return NotFound();
            }

            var subjectFile = await _context.Files
                .FirstOrDefaultAsync(m => m.SubjectId == Subjectid);
            if (subjectFile == null)
            {
                return NotFound();
            }

            return Ok(subjectFile);
        }
        // POST: SubjectFiles/Create
        [HttpPost("Create")]
        public async Task<IActionResult> Create([FromBody] SubjectFileViewModel subjectFile)
        {
            _logger.LogInformation("Processing request {0}", Request.Path);
            if (ModelState.IsValid)
            {
                SubjectFile file = new SubjectFile { Id = subjectFile.Id, SubjectId = subjectFile.SubjectId };
                _context.Add(file);
                await _context.SaveChangesAsync();
                return Ok(file);
            }
            return BadRequest("Не удалось выполнить запрос");
        }

        // GET: SubjectFiles/Delete/5
        [HttpDelete("Delete/{id}")]
        public async Task<IActionResult> Delete(Guid? id)
        {
            _logger.LogInformation("Processing request {0}", Request.Path);
            if (id == null)
            {
                return NotFound();
            }

            var subjectFile = await _context.Files
                .FirstOrDefaultAsync(m => m.Id == id);
            _context.Remove(subjectFile);
            if (subjectFile == null)
            {
                return NotFound();
            }

            return Ok("Удалено!");
        }
    }
}
