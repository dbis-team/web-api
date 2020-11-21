using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using EducationOnlinePlatform.Models;

namespace EducationOnlinePlatform.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SubjectContoller : ControllerBase
    {
        private ApplicationContext db = new ApplicationContext();

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
                return System.Text.Json.JsonSerializer.Serialize<Result>(new Result { success = false, description = "Id Not found" });
            }
            var subject = db.Subjects.FirstOrDefault(e => e.Id == id);
            if(subject == null)
            {
                return System.Text.Json.JsonSerializer.Serialize<Result>(new Result { success = false, description = "Subject Not found" });
            }
            return System.Text.Json.JsonSerializer.Serialize<Subject>(subject);
        }

        // POST: subject/add
        [HttpPost]
        [Route("add")]
        public string AddSubject([FromBody][Bind("Name,EducationSetId")] Subject subject)
        {
            var savedRows = 0;
            if (ModelState.IsValid)
            {
                var subjects = db.Subjects;
                subjects.Add(subject);
                savedRows = db.SaveChanges();
            }
            return System.Text.Json.JsonSerializer.Serialize<Result>(new Result { success = savedRows > 0, description = "Saved rows" + savedRows });
        }

        // PUT: subject/update/5
        [HttpPut("update/{id}")]
        public string UpdateSubject(Guid id, [FromBody][Bind("Name,EducationSetId")] Subject subjectChanged)
        {
            if (id == null)
            {
                return System.Text.Json.JsonSerializer.Serialize<Result>(new Result { success = false, description = "Id Not found" });
            }
            var subject = db.Subjects.FirstOrDefault(s => s.Id == id);
            if (subject != null)
            {
                if (subject.Name != subjectChanged.Name)
                {
                    subject.Name = subjectChanged.Name;
                }
                var savedRows = db.SaveChanges();
                return System.Text.Json.JsonSerializer.Serialize<Result>(new Result { success = savedRows > 0, description = "Saved rows" + savedRows });
            }
            else
            {
                return System.Text.Json.JsonSerializer.Serialize<Result>(new Result { success = false, description = "Subject Not found" });
            }
        }

        // DELETE: subject/delete/5
        [HttpDelete("delete/{id}")]
        public string DeleteSubject(Guid id)
        {
            if (id == null)
            {
                return System.Text.Json.JsonSerializer.Serialize<Result>(new Result { success = false, description = "Id Not found" });
            }
            var subject = db.Subjects.FirstOrDefault(s => s.Id == id);
            if (subject != null)
            {
                db.Subjects.Remove(subject);
            }
            else
            {
                return System.Text.Json.JsonSerializer.Serialize<Result>(new Result { success = false, description = "Subject Not found" });
            }
            var savedRows = db.SaveChanges();
            return System.Text.Json.JsonSerializer.Serialize<Result>(new Result { success = savedRows > 0, description = "Saved rows" + savedRows });
        }
    }
}