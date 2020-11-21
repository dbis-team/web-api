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
        public string GetUser(Guid? id)
        {
            if (id == null)
            {
                return System.Text.Json.JsonSerializer.Serialize<Result>(new Result { success = false, description = "Id Not found" });
            }
            var user = db.Users.FirstOrDefault(u => u.Id == id);
            if (user == null)
            {
                return System.Text.Json.JsonSerializer.Serialize<Result>(new Result { success = false, description = "User Not found" });
            }
            return System.Text.Json.JsonSerializer.Serialize<User>(user);
        }

        // POST: User/register
        [HttpPost]
        [Route("register")]
        public string RegisterUser([FromBody][Bind("UserName,Password,Email,IsSysAdmin")] User user)
        {
            var savedRows = 0;
            if (ModelState.IsValid)
            {
                var users = db.Users;
                if (users.FirstOrDefault(u => u.Email == user.Email) == null)
                {
                    users.Add(user);
                    savedRows = db.SaveChanges();
                }
            }
            return System.Text.Json.JsonSerializer.Serialize<Result>(new Result { success = savedRows > 0, description = "Saved Rows " + savedRows });
        }

        // PUT: /User/update/5
        [HttpPut("update/{id}")]
        public string UpdateUser(Guid id, [FromBody][Bind("UserName,Password,Email,IsSysAdmin")] User userChanged)
        {
            if (id == null)
            {
                return System.Text.Json.JsonSerializer.Serialize<Result>(new Result { success = false, description = "Id Not found" });
            }

            var user = db.Users.FirstOrDefault(u => u.Id == id);
            if (user == null)
            {
                return System.Text.Json.JsonSerializer.Serialize<Result>(new Result { success = false, description = "User Not found" });
            }
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
            if (user.IsSysAdmin != userChanged.IsSysAdmin)
            {
                user.IsSysAdmin = userChanged.IsSysAdmin;
            }
            var savedRows = db.SaveChanges();
            return System.Text.Json.JsonSerializer.Serialize<Result>(new Result { success = savedRows > 0, description = "Saved rows " + savedRows });
        }

        // DELETE: user/delete/5
        [HttpDelete("delete/{id}")]
        public string DeleteUser(Guid id)
        {
            if (id == null)
            {
                return System.Text.Json.JsonSerializer.Serialize<Result>(new Result { success = false, description = "Id Not found" });
            }
            var user = db.Users.FirstOrDefault(u => u.Id == id);
            if (user == null)
            {
                return System.Text.Json.JsonSerializer.Serialize<Result>(new Result { success = false, description = "User Not found" });
            }
            db.Users.Remove(user);
            var savedRows = db.SaveChanges();
            return System.Text.Json.JsonSerializer.Serialize<Result>(new Result { success = savedRows > 0, description = "Saved rows " + savedRows });
        }
    }
}