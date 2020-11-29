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

namespace EducationOnlinePlatform.Controllers
{
    [EnableCors("CorsApi")]
    [ApiController]
    [Route("[controller]")]
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
        public IActionResult AddSubject([FromBody][Bind("Name,EducationSetId")] Subject subject)
        {
            if (ModelState.IsValid)
            {
                var subjects = db.Subjects;
                subjects.Add(subject);
                return Ok(new Result { Status = HttpStatusCode.OK, Message = "Saved rows " + db.SaveChanges() }.ToString());
            }
            else
            {
                return NotFound(new Result { Status = HttpStatusCode.NotFound, Message = "Not correct Json model" }.ToString());
            }
        }

        // PUT: subject/update/5
        [HttpPut("update/{id}")]
        public IActionResult UpdateSubject(Guid id, [FromBody][Bind("Name,EducationSetId")] Subject subjectChanged)
        {
            if (id == null)
            {
                return NotFound(new Result { Status = HttpStatusCode.NotFound, Message = "Id Not found" }.ToString());
            }
            var subject = db.Subjects.FirstOrDefault(s => s.Id == id);
            if (subject != null)
            {
                if (subject.Name != subjectChanged.Name)
                {
                    subject.Name = subjectChanged.Name;
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
    }
}