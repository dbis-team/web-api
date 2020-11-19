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
    public class EducationSetContoller : ControllerBase
    {
        private ApplicationContext db = new ApplicationContext();

        [HttpGet]
        public string EducationSets()
        {
            return JsonConvert.SerializeObject(db.EducationSets.ToList(), Formatting.Indented);
        }

        // GET: EducationSet/5
        [HttpGet("{id}")]
        public string EducationSet(Guid id)
        {
            var educationSet = db.EducationSets.FirstOrDefault(e => e.Id == id);
            return System.Text.Json.JsonSerializer.Serialize<EducationSet>(educationSet);
        }

        // POST: educationSet/register
        [HttpPost]
        [Route("add")]
        public bool AddEducationSet([FromBody]EducationSet educationSet)
        {
            var educationSets = db.EducationSets;
            if (educationSets.FirstOrDefault(e => e.Name == educationSet.Name) == null)
            {
                educationSets.Add(educationSet);
            }
            return db.SaveChanges() > 0;
        }

        // PUT: /educationSet/update/5
        [HttpPut("update/{id}")]
        public bool UpdateEducationSet(Guid id, [FromBody] EducationSet educationSetChanged)
        {
            var educationSet = db.EducationSets.FirstOrDefault(e => e.Id == id);
            if (educationSet != null)
            {
                if (educationSet.Name != educationSetChanged.Name)
                {
                    educationSet.Name = educationSetChanged.Name;
                }
            }
            return db.SaveChanges() > 0;
        }

        // DELETE: educationSet/delete/5
        [HttpDelete("delete/{id}")]
        public bool DeleteEducationSet(Guid id)
        {
            var educationSet = db.EducationSets.FirstOrDefault(e => e.Id == id);
            if (educationSet != null)
            {
                db.EducationSets.Remove(educationSet);
            }
            return db.SaveChanges() > 0;
        }
    }
}