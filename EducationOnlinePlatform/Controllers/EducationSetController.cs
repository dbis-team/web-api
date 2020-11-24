﻿using Microsoft.AspNetCore.Http;
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
    public class EducationSetContoller : ControllerBase
    {
        private readonly ApplicationContext db = new ApplicationContext();

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
                return (new Result { Status = HttpStatusCode.NotFound, Message = "Id Not found" }).ToString();
            }
            var educationSet = db.EducationSets.FirstOrDefault(e => e.Id == id);
            if (educationSet == null)
            {
                return (new Result { Status = HttpStatusCode.NotFound, Message = "Education Set Not found" }).ToString();
            }
            return System.Text.Json.JsonSerializer.Serialize<EducationSet>(educationSet);
        }

        // POST: educationSet/register
        [HttpPost]
        [Route("add")]
        public string AddEducationSet([FromBody][Bind("Name")] EducationSet educationSet)
        {
            if (ModelState.IsValid)
            {
                var educationSets = db.EducationSets;
                if (educationSets.FirstOrDefault(e => e.Name == educationSet.Name) == null)
                {
                    educationSets.Add(educationSet);
                }
                else
                {
                    return (new Result { Status = HttpStatusCode.NotFound, Message = "Name Not Unique" }).ToString();
                }
                return (new Result { Status = HttpStatusCode.OK, Message = "Saved rows " + db.SaveChanges() }).ToString();
            }
            else
            {
                return (new Result { Status = HttpStatusCode.NotFound, Message = "Not correct Json model" }).ToString();
            }
        }

        // PUT: /educationSet/update/5
        [HttpPut("update/{id}")]
        public string UpdateEducationSet(Guid id, [FromBody][Bind("Name")] EducationSet educationSetChanged)
        {
            if(id == null)
            {
                return (new Result { Status = HttpStatusCode.NotFound, Message = "Id Not found" }).ToString();
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
                return (new Result { Status = HttpStatusCode.NotFound, Message = "Education Set Not found" }).ToString();
            }
            return (new Result { Status = HttpStatusCode.OK, Message = "Saved rows " + db.SaveChanges() }).ToString();
        }

        // DELETE: educationSet/delete/5
        [HttpDelete("delete/{id}")]
        public string DeleteEducationSet(Guid id)
        {
            if(id == null)
            {
                return (new Result { Status = HttpStatusCode.NotFound, Message = "Id Not found" }).ToString();
            }
            var educationSet = db.EducationSets.FirstOrDefault(e => e.Id == id);
            if (educationSet != null)
            {
                db.EducationSets.Remove(educationSet);
            }
            else
            {
                return (new Result { Status = HttpStatusCode.NotFound, Message = "Education Set Not found" }).ToString();
            }
            return (new Result { Status = HttpStatusCode.OK, Message = "Saved rows " + db.SaveChanges() }).ToString();
        }
    }
}