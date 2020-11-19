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
            var subject = db.Subjects.FirstOrDefault(e => e.Id == id);
            return System.Text.Json.JsonSerializer.Serialize<Subject>(subject);
        }

        // POST: subject/add
        [HttpPost]
        [Route("add")]
        public bool AddSubject([FromBody] Subject subject)
        {
            var subjects = db.Subjects;
            subjects.Add(subject);
            return db.SaveChanges() > 0;
        }

        // PUT: subject/update/5
        [HttpPut("update/{id}")]
        public bool UpdateSubject(Guid id, [FromBody] Subject subjectChanged)
        {
            var subject = db.Subjects.FirstOrDefault(s => s.Id == id);
            if (subject != null)
            {
                if (subject.Name != subjectChanged.Name)
                {
                    subject.Name = subjectChanged.Name;
                }
            }
            return db.SaveChanges() > 0;
        }

        // DELETE: subject/delete/5
        [HttpDelete("delete/{id}")]
        public bool DeleteSubject(Guid id)
        {
            var subject = db.Subjects.FirstOrDefault(s => s.Id == id);
            if (subject != null)
            {
                db.Subjects.Remove(subject);
            }
            return db.SaveChanges() > 0;
        }
    }
}