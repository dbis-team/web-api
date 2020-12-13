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

        // GET: SubjectFiles/Details/5
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
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromBody] SubjectFile subjectFile)
        {
            if (ModelState.IsValid)
            {
                SubjectFile file = new SubjectFile { Id = (Guid)subjectFile.Id };
                _context.Add(file);
                await _context.SaveChangesAsync();
                return Ok(file);
            }
            return BadRequest("Не удалось выполнить инсерт");
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
