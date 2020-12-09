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

namespace EducationOnlinePlatform.Controllers
{
    [EnableCors("CorsApi")]
    [ApiController]
    [Route("Subject")]
    public class SubjectContoller : ControllerBase
    {
        private readonly ApplicationContext db = new ApplicationContext();

        [HttpGet]
        public IActionResult Subjects()
        {
            return Ok(JsonConvert.SerializeObject(db.Subjects.ToList(), Formatting.Indented));
        }

        // GET: Subject/5
        [HttpGet("{id}")]
        public IActionResult GetSubject(Guid id)
        {
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
        public IActionResult AddSubject([FromBody] AddSubjectViewModel subjectAdd)
        {
            if (ModelState.IsValid)
            {
                var subjects = db.Subjects;
                subjects.Add(new Subject {Name = subjectAdd.Name, EducationSetId = subjectAdd.EducationSetId, Description = subjectAdd.Description });
                return Ok(new Result { Status = HttpStatusCode.OK, Message = "Saved rows " + db.SaveChanges() }.ToString());
            }
            else
            {
                return NotFound(new Result { Status = HttpStatusCode.NotFound, Message = "Not correct Json model" }.ToString());
            }
        }

        // PUT: subject/update/5
        [HttpPut("update/{id}")]
        public IActionResult UpdateSubject(Guid id, [FromBody] UpdateSubject subjectUpdate)
        {
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
                return Ok(new Result { Status = HttpStatusCode.OK, Message = "Saved rows" + db.SaveChanges() }.ToString());
            }
            else
            {
                return NotFound(new Result { Status = HttpStatusCode.NotFound, Message = "Subject Not found" }.ToString());
            }
        }

        // DELETE: subject/delete/5
        [HttpDelete("delete/{id}")]
        public IActionResult DeleteSubject(Guid id)
        {
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
            return Ok(new Result { Status = HttpStatusCode.OK, Message = "Saved rows " + db.SaveChanges() }.ToString());
        }
        [HttpGet("EducationSet/{EducationSetId}")]
        public IActionResult GetSubjectInEducationSet(Guid? EducationSetId)
        {
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