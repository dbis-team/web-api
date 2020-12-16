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

namespace EducationOnlinePlatform.Controllers
{
    [EnableCors("CorsApi")]
    [ApiController]
    [Route("Files")]
    public class SubjectFilesController : Controller
    {
        private readonly ApplicationContext _context;

        public SubjectFilesController(ApplicationContext context)
        {
            _context = context;
        }

        // GET: SubjectFiles
        [HttpGet]
        public async Task<IActionResult> Index()
        {
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
        // POST: SubjectFiles/Create
        [HttpPost("Create")]
        public async Task<IActionResult> Create([FromBody] SubjectFileViewModel subjectFile)
        {
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
