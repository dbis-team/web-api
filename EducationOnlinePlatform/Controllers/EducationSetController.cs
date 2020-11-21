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
            if (id == null)
            {
                return System.Text.Json.JsonSerializer.Serialize<Result>(new Result { success = false, description = "Id Not found" });
            }
            var educationSet = db.EducationSets.FirstOrDefault(e => e.Id == id);
            if (educationSet == null)
            {
                return System.Text.Json.JsonSerializer.Serialize<Result>(new Result { success = false, description = "Education Set Not found" });
            }
            return System.Text.Json.JsonSerializer.Serialize<EducationSet>(educationSet);
        }

        // POST: educationSet/register
        [HttpPost]
        [Route("add")]
        public string AddEducationSet([FromBody]EducationSet educationSet)
        {
            var savedRows = 0;
            if (ModelState.IsValid)
            {
                var educationSets = db.EducationSets;
                if (educationSets.FirstOrDefault(e => e.Name == educationSet.Name) == null)
                {
                    educationSets.Add(educationSet);
                }
                else
                {
                    return System.Text.Json.JsonSerializer.Serialize<Result>(new Result { success = false, description = "Name Not Unique" });
                }
            }
            savedRows = db.SaveChanges();
            return System.Text.Json.JsonSerializer.Serialize<Result>(new Result { success = savedRows > 0, description = "Saved rows" + savedRows });
        }

        // PUT: /educationSet/update/5
        [HttpPut("update/{id}")]
        public string UpdateEducationSet(Guid id, [FromBody] EducationSet educationSetChanged)
        {
            var savedRows = 0;
            if(id == null)
            {
                return System.Text.Json.JsonSerializer.Serialize<Result>(new Result { success = false, description = "Id Not found" });
            }
            var educationSet = db.EducationSets.FirstOrDefault(e => e.Id == id);
            if (educationSet != null)
            {
                if (educationSet.Name != educationSetChanged.Name)
                {
                    educationSet.Name = educationSetChanged.Name;
                }
            }
            else
            {
                return System.Text.Json.JsonSerializer.Serialize<Result>(new Result { success = false, description = "Education Set Not found" });
            }
            savedRows = db.SaveChanges();
            return System.Text.Json.JsonSerializer.Serialize<Result>(new Result { success = savedRows > 0, description = "Saved rows" + savedRows });
        }

        // DELETE: educationSet/delete/5
        [HttpDelete("delete/{id}")]
        public string DeleteEducationSet(Guid id)
        {
            var savedRows = 0;
            if(id == null)
            {
                return System.Text.Json.JsonSerializer.Serialize<Result>(new Result { success = false, description = "Id Not found" });
            }
            var educationSet = db.EducationSets.FirstOrDefault(e => e.Id == id);
            if (educationSet != null)
            {
                db.EducationSets.Remove(educationSet);
            }
            else
            {
                return System.Text.Json.JsonSerializer.Serialize<Result>(new Result { success = false, description = "Education Set Not found" });
            }
            savedRows = db.SaveChanges();
            return System.Text.Json.JsonSerializer.Serialize<Result>(new Result { success = savedRows > 0, description = "Education Set Not found" });
        }
    }
}