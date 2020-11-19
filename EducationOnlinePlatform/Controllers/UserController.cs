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
    public class UserController : ControllerBase
    {
        private ApplicationContext db = new ApplicationContext();

        [HttpGet]
        public string GetUsers()
        {
            return JsonConvert.SerializeObject(db.Users.ToList(), Formatting.Indented);
        }

        // GET: User/5
        [HttpGet("{id}")]
        public string GetUser(Guid id)
        {
            var user = db.Users.FirstOrDefault(u => u.Id == id);
            return System.Text.Json.JsonSerializer.Serialize<User>(user);
        }

        // POST: User/register
        [HttpPost]
        [Route("register")]
        public bool RegisterUser([FromBody]User user)
        {
            var users = db.Users;
            if (users.FirstOrDefault(u => u.Email == user.Email) == null)
            {
                users.Add(user);
            }
            return db.SaveChanges() > 0;
        }

        // PUT: /User/update/5
        [HttpPut("update/{id}")]
        public bool UpdateUser(Guid id, [FromBody] User userChanged)
        {
            var user = db.Users.FirstOrDefault(u => u.Id == id);
            if (user != null)
            {
                if (userChanged.UserName != user.UserName)
                {
                    user.UserName = userChanged.UserName;
                }
                if (userChanged.Password != user.Password)
                {
                    user.Password = userChanged.Password;
                }
                if (user.Email != userChanged.Email)
                {
                    user.Email = userChanged.Email;
                }
            }
            return db.SaveChanges() > 0;
        }

        // DELETE: user/delete/5
        [HttpDelete("delete/{id}")]
        public bool DeleteUser(Guid id)
        {
            var user = db.Users.FirstOrDefault(u => u.Id == id);
            if (user != null)
            {
                db.Users.Remove(user);
            }
            return db.SaveChanges() > 0;
        }
    }
}