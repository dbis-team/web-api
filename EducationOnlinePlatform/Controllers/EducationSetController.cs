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
    [Route("[controller]")]
    public class EducationSetContoller : ControllerBase
    {
        private readonly ApplicationContext db = new ApplicationContext();

        [HttpGet]
        public IActionResult EducationSets()
        {
            return Ok(JsonConvert.SerializeObject(db.EducationSets.ToList(), Formatting.Indented));
        }

        // GET: EducationSet/5
        [HttpGet("{id}")]
        public IActionResult EducationSet(Guid id)
        {
            if (id == null)
            {
                return NotFound(new Result { Status = HttpStatusCode.NotFound, Message = "Id Not found" }.ToString());
            }
            var educationSet = db.EducationSets.FirstOrDefault(e => e.Id == id);
            if (educationSet == null)
            {
                return NotFound(new Result { Status = HttpStatusCode.NotFound, Message = "Education Set Not found" }.ToString());
            }
            return Ok(System.Text.Json.JsonSerializer.Serialize<EducationSet>(educationSet));
        }

        // POST: educationSet/add
        [HttpPost]
        [Route("add")]
        public IActionResult AddEducationSet([FromBody] AddEducationSet educationSetAdd)
        {
            if (ModelState.IsValid)
            {
                var educationSets = db.EducationSets;
                if (educationSets.FirstOrDefault(e => e.Name == educationSetAdd.Name) == null)
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
        public IActionResult UpdateEducationSet(Guid id, [FromBody] EducationSetUpdate educationSetUpdate)
        {
            if(id == null)
            {
                return NotFound(new Result { Status = HttpStatusCode.NotFound, Message = "Id Not found" }.ToString());
            }
            var educationSet = db.EducationSets.FirstOrDefault(e => e.Id == id);
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
        public IActionResult DeleteEducationSet(Guid id)
        {
            if(id == null)
            {
                return NotFound(new Result { Status = HttpStatusCode.NotFound, Message = "Id Not found" }.ToString());
            }
            var educationSet = db.EducationSets.FirstOrDefault(e => e.Id == id);
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