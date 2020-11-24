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
    [ApiController]
    [Route("[controller]")]
    public class SubjectContoller : ControllerBase
    {
        private readonly ApplicationContext db = new ApplicationContext();

        [HttpGet]
        public string Subjects()
        {
            return JsonConvert.SerializeObject(db.Subjects.ToList(), Formatting.Indented);
        }

        // GET: Subject/5
        [HttpGet("{id}")]
        public string GetSubject(Guid id)
        {
            if (id == null)
            {
                return (new Result { Status = HttpStatusCode.NotFound, Message = "Id Not found" }).ToString();
            }
            var subject = db.Subjects.FirstOrDefault(e => e.Id == id);
            if(subject == null)
            {
                return (new Result { Status = HttpStatusCode.NotFound, Message = "Subject Not found" }).ToString();
            }
            return System.Text.Json.JsonSerializer.Serialize<Subject>(subject);
        }

        // POST: subject/add
        [HttpPost]
        [Route("add")]
        public string AddSubject([FromBody][Bind("Name,EducationSetId")] Subject subject)
        {
            if (ModelState.IsValid)
            {
                var subjects = db.Subjects;
                subjects.Add(subject);
                return (new Result { Status = HttpStatusCode.OK, Message = "Saved rows " + db.SaveChanges() }).ToString();
            }
            else
            {
                return (new Result { Status = HttpStatusCode.NotFound, Message = "Not correct Json model" }).ToString();
            }
        }

        // PUT: subject/update/5
        [HttpPut("update/{id}")]
        public string UpdateSubject(Guid id, [FromBody][Bind("Name,EducationSetId")] Subject subjectChanged)
        {
            if (id == null)
            {
                return (new Result { Status = HttpStatusCode.NotFound, Message = "Id Not found" }).ToString();
            }
            var subject = db.Subjects.FirstOrDefault(s => s.Id == id);
            if (subject != null)
            {
                if (subject.Name != subjectChanged.Name)
                {
                    subject.Name = subjectChanged.Name;
                }
                return (new Result { Status = HttpStatusCode.OK, Message = "Saved rows" + db.SaveChanges() }).ToString();
            }
            else
            {
                return(new Result { Status = HttpStatusCode.NotFound, Message = "Subject Not found" }).ToString();
            }
        }

        // DELETE: subject/delete/5
        [HttpDelete("delete/{id}")]
        public string DeleteSubject(Guid id)
        {
            if (id == null)
            {
                return (new Result { Status = HttpStatusCode.NotFound, Message = "Id Not found" }).ToString();
            }
            var subject = db.Subjects.FirstOrDefault(s => s.Id == id);
            if (subject != null)
            {
                db.Subjects.Remove(subject);
            }
            else
            {
                return (new Result { Status = HttpStatusCode.NotFound, Message = "Subject Not found" }).ToString();
            }
            return (new Result { Status = HttpStatusCode.OK, Message = "Saved rows " + db.SaveChanges() }).ToString();
        }
    }
}