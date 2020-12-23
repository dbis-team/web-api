using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using EducationOnlinePlatform.Models;
using System.Net;
using Microsoft.AspNetCore.Cors;
using EducationOnlinePlatform.ViewModels;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;

namespace EducationOnlinePlatform.Controllers
{
    [EnableCors("CorsApi")]
    [ApiController]
    [Route("EducationSet")]
    public class EducationSetContoller : ControllerBase
    {
        ApplicationContext db;

        private readonly ILogger _logger;

        public EducationSetContoller(ApplicationContext context, ILogger<EducationSetContoller> logger)
        {
            db = context;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> EducationSets()
        {
            _logger.LogInformation("Processing request {0}", Request.Path);
            return Ok(JsonConvert.SerializeObject( await db.EducationSets.ToListAsync(), Formatting.Indented));
        }

        // GET: EducationSet/5
        [HttpGet("{id}")]
        public async Task<IActionResult> EducationSet(Guid? id)
        {
            _logger.LogInformation("Processing request {0}", Request.Path);
            if (id == null)
            {
                return NotFound(new Result { Status = HttpStatusCode.NotFound, Message = "Id Not found" }.ToString());
            }
            var educationSet = await db.EducationSets.FirstOrDefaultAsync(e => e.Id == id);
            if (educationSet == null)
            {
                return NotFound(new Result { Status = HttpStatusCode.NotFound, Message = "Education Set Not found" }.ToString());
            }
            return Ok(System.Text.Json.JsonSerializer.Serialize<EducationSet>(educationSet));
        }

        // POST: educationSet/add
        [HttpPost]
        [Route("add")]
        public async Task<IActionResult> AddEducationSet([FromBody] AddEducationSet educationSetAdd)
        {
            _logger.LogInformation("Processing request {0}", Request.Path);
            if (ModelState.IsValid)
            {
                var educationSets = db.EducationSets;
                var educ = await educationSets.FirstOrDefaultAsync();
                if (educ == null)
                {
                    educationSets.Add( new EducationSet { Name = educationSetAdd.Name, Description = educationSetAdd.Description });
                }
                else
                {
                    return NotFound(new Result { Status = HttpStatusCode.NotFound, Message = "Name Not Unique" }.ToString());
                }
                return Ok(new Result { Status = HttpStatusCode.OK, Message = "Saved rows " + db.SaveChanges() }.ToString());
            }
            else
            {
                return NotFound(new Result { Status = HttpStatusCode.NotFound, Message = "Not correct Json model" }.ToString());
            }
        }

        // PUT: /educationSet/update/5
        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateEducationSet(Guid id, [FromBody] EducationSetUpdate educationSetUpdate)
        {
            _logger.LogInformation("Processing request {0}", Request.Path);
            if (id == null)
            {
                return NotFound(new Result { Status = HttpStatusCode.NotFound, Message = "Id Not found" }.ToString());
            }
            var educationSet = await db.EducationSets.FirstOrDefaultAsync(e => e.Id == id);
            if (educationSet != null)
            {
                if (educationSet.Name != educationSetUpdate.Name)
                {
                    educationSet.Name = educationSetUpdate.Name;
                }
                if (educationSet.Description != educationSetUpdate.Description)
                {
                    educationSet.Description = educationSetUpdate.Description;
                }
            }
            else
            {
                return NotFound(new Result { Status = HttpStatusCode.NotFound, Message = "Education Set Not found" }.ToString());
            }
            return Ok(new Result { Status = HttpStatusCode.OK, Message = "Saved rows " + db.SaveChanges() }.ToString());
        }

        // DELETE: educationSet/delete/5
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteEducationSet(Guid id)
        {
            _logger.LogInformation("Processing request {0}", Request.Path);
            if (id == null)
            {
                return NotFound(new Result { Status = HttpStatusCode.NotFound, Message = "Id Not found" }.ToString());
            }
            var educationSet = await db.EducationSets.FirstOrDefaultAsync(e => e.Id == id);
            if (educationSet != null)
            {
                db.EducationSets.Remove(educationSet);
            }
            else
            {
                return NotFound(new Result { Status = HttpStatusCode.NotFound, Message = "Education Set Not found" }.ToString());
            }
            return Ok(new Result { Status = HttpStatusCode.OK, Message = "Saved rows " + db.SaveChanges() }.ToString());
        }
    }
}