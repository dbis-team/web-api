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

namespace EducationOnlinePlatform.Controllers
{
    [EnableCors("CorsApi")]
    [ApiController]
    [Route("Subject")]
    public class SubjectContoller : ControllerBase
    {
        ApplicationContext db;

        private readonly ILogger _logger;
        public SubjectContoller(ApplicationContext context, ILogger<SubjectContoller> logger)
        {
            db = context;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Subjects()
        {
            _logger.LogInformation("Processing request {0}", Request.Path);
            return Ok(JsonConvert.SerializeObject(db.Subjects.ToList(), Formatting.Indented));
        }

        // GET: Subject/5
        [HttpGet("{id}")]
        public IActionResult GetSubject(Guid id)
        {
            _logger.LogInformation("Processing request {0}", Request.Path);
            if (id == null)
            {
                return NotFound(new Result { Status = HttpStatusCode.NotFound, Message = "Id Not found" }.ToString());
            }
            var subject = db.Subjects.FirstOrDefault(e => e.Id == id);
            if(subject == null)
            {
                return NotFound(new Result { Status = HttpStatusCode.NotFound, Message = "Subject Not found" }.ToString());
            }
            return Ok(System.Text.Json.JsonSerializer.Serialize<Subject>(subject));
        }

        // POST: subject/add
        [HttpPost]
        [Route("add")]
        public async Task<IActionResult> AddSubject([FromBody] AddSubjectViewModel subjectAdd)
        {
            _logger.LogInformation("Processing request {0}", Request.Path);
            if (ModelState.IsValid)
            {
                var subjects = db.Subjects;
                var subject = new Subject
                {
                    Name = subjectAdd.Name,
                    EducationSetId = subjectAdd.EducationSetId,
                    Description = subjectAdd.Description
                };
                subjects.Add(subject);
                await db.SaveChangesAsync();
                return Ok(JsonConvert.SerializeObject(subject, Formatting.Indented));
            }
            else
            {
                return NotFound(new Result { Status = HttpStatusCode.NotFound, Message = "Not correct Json model" }.ToString());
            }
        }

        // PUT: subject/update/5
        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateSubject(Guid id, [FromBody] UpdateSubject subjectUpdate)
        {
            _logger.LogInformation("Processing request {0}", Request.Path);
            if (id == null)
            {
                return NotFound(new Result { Status = HttpStatusCode.NotFound, Message = "Id Not found" }.ToString());
            }
            var subject = db.Subjects.FirstOrDefault(s => s.Id == id);
            if (subject != null)
            {
                if (subject.Name != subjectUpdate.Name)
                {
                    subject.Name = subjectUpdate.Name;
                }
                if (subject.Description != subjectUpdate.Description)
                {
                    subject.Description = subjectUpdate.Description;
                }
                await db.SaveChangesAsync();
                return Ok(new Result { Status = HttpStatusCode.OK, Message = "Subject was Updated" }.ToString());
            }
            else
            {
                return NotFound(new Result { Status = HttpStatusCode.NotFound, Message = "Subject Not found" }.ToString());
            }
        }

        // DELETE: subject/delete/5
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteSubject(Guid id)
        {
            _logger.LogInformation("Processing request {0}", Request.Path);
            if (id == null)
            {
                return NotFound(new Result { Status = HttpStatusCode.NotFound, Message = "Id Not found" }.ToString());
            }
            var subject = db.Subjects.FirstOrDefault(s => s.Id == id);
            if (subject != null)
            {
                db.Subjects.Remove(subject);
            }
            else
            {
                return NotFound(new Result { Status = HttpStatusCode.NotFound, Message = "Subject Not found" }.ToString());
            }
            await db.SaveChangesAsync();
            return Ok(new Result { Status = HttpStatusCode.OK, Message = "Saved was deleted" }.ToString());
        }
        [HttpGet("EducationSet/{EducationSetId}")]
        public IActionResult GetSubjectInEducationSet(Guid? EducationSetId)
        {
            _logger.LogInformation("Processing request {0}", Request.Path);
            if (ModelState.IsValid)
            {
                var subjects = (from sub in db.Subjects
                               join educSet in db.EducationSets on sub.EducationSetId equals educSet.Id
                               where educSet.Id == EducationSetId
                               select new
                               {
                                   Id = sub.Id,
                                   Name = sub.Name,
                                   Description = sub.Description,
                                   EducationSetId = educSet.Id,
                                   EducationSetName = educSet.Name
                               }).ToList();
                return Ok(JsonConvert.SerializeObject(subjects, Formatting.Indented));
            }
            return NotFound();
        }
    }
}