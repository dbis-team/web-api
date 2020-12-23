using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EducationOnlinePlatform;
using EducationOnlinePlatform.Models;
using Newtonsoft.Json;
using EducationOnlinePlatform.ViewModels;
using System.Net;
using Microsoft.AspNetCore.Cors;
using Microsoft.Extensions.Logging;

namespace EducationOnlinePlatform.Controllers
{
    [EnableCors("CorsApi")]
    [ApiController]
    [Route("[controller]")]
    public class UserInEducationSetsController : Controller
    {
        private readonly ApplicationContext _context;
        private readonly ILogger _logger;

        public UserInEducationSetsController(ApplicationContext context, ILogger<EducationSetContoller> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: UserInEducationSets
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            _logger.LogInformation("Processing request {0}", Request.Path);
            var applicationContext = _context.UserInEducationSet.Include(u => u.EducationSet).Include(u => u.User);
            return View(await applicationContext.ToListAsync());
        }

        // GET: UserInEducationSets/EducationSet/5
        [HttpGet("EducationSet/{EducationSetId}")]
        public async Task<IActionResult> GetUsersInEducationSets(Guid? EducationSetId)
        {
            _logger.LogInformation("Processing request {0}", Request.Path);
            if (EducationSetId == null)
            {
                return NotFound();
            }

            var userInEducationSets = await (_context.UserInEducationSet
                .Include(u => u.EducationSet)
                .Include(u => u.User)
                .Where(m => m.EducationSetId == EducationSetId))
                .ToListAsync();
            if (userInEducationSets == null)
            {
                return NotFound();
            }

            return Ok(JsonConvert.SerializeObject(userInEducationSets));
        }

        [HttpPost("Add")]
        public async Task<IActionResult> Add([FromBody] UserInEducationSetViewModel userInEducationSet)
        {
            _logger.LogInformation("Processing request {0}", Request.Path);
            if (ModelState.IsValid)
            {
                var userInEducationSetNew = new UserInEducationSet{ EducationSetId = userInEducationSet.EducationSetId, UserId = userInEducationSet.UserId};
                _context.Add(userInEducationSetNew);
                await _context.SaveChangesAsync();
                return Ok(JsonConvert.SerializeObject(userInEducationSetNew));
            }
            return BadRequest(new Result { Status = HttpStatusCode.BadRequest, Message = "Model is no correct" }.ToString());
        }

        // POST: UserInEducationSets/Delete/5
        [HttpPost("Delete")]
        public async Task<IActionResult> DeleteConfirmed([FromBody] UserInEducationSetViewModel userInEducationSet)
        {
            _logger.LogInformation("Processing request {0}", Request.Path);
            var userInEducationSetDeleted = await _context.UserInEducationSet.FirstOrDefaultAsync(u => u.UserId == userInEducationSet.UserId && u.EducationSetId == userInEducationSet.EducationSetId);
            _context.UserInEducationSet.Remove(userInEducationSetDeleted);
            await _context.SaveChangesAsync();
            return Ok();
        }
    }
}
