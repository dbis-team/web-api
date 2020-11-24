using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using EducationOnlinePlatform.Models;
using System.Net;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text.Json;
using System.Text.Json.Serialization;
using EducationOnlinePlatform.ViewModels;
using System.Text;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Cors;

namespace EducationOnlinePlatform.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly ApplicationContext db = new ApplicationContext();

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
                return (new Result { Status = HttpStatusCode.NotFound, Message = "Id Not found" }).ToString();
            }
            var user = db.Users.FirstOrDefault(u => u.Id == id);
            if (user == null)
            {
                return (new Result { Status = HttpStatusCode.NotFound, Message = "User Not found" }).ToString();
            }
            return System.Text.Json.JsonSerializer.Serialize<User>(user);
        }

        // POST: User/register
        [HttpPost]
        [Route("register")]
        public string RegisterUser([FromBody][Bind("UserName,Password,Email,IsSysAdmin")] User user)
        {
            if (ModelState.IsValid)
            {
                var users = db.Users;
                if (users.FirstOrDefault(u => u.Email == user.Email) == null)
                {
                    users.Add(user);

                    user.Password = HeshMD5(user.Password);
                    return new Result { Status = HttpStatusCode.OK, Message = "Saved Rows " + db.SaveChanges() }.ToString();
                }
                else
                {
                    return new Result { Status = HttpStatusCode.NotFound, Message = "Email is busy" }.ToString();
                }
            }
            else
            {
                return (new Result { Status = HttpStatusCode.NotFound, Message = "Not correct Json model"}).ToString();
            }
        }

        // PUT: /User/update/5
        [HttpPut("update/{id}")]
        public string UpdateUser(Guid? id, [FromBody][Bind("UserName,Password,Email,IsSysAdmin")] User userChanged)
        {
            if (id == null)
            {
                return (new Result { Status = HttpStatusCode.NotFound, Message = "Id Not found" }).ToString();
            }

            var user = db.Users.FirstOrDefault(u => u.Id == id);
            if (user == null)
            {
                return (new Result { Status = HttpStatusCode.NotFound, Message = "User Not found" }).ToString();
            }
            if (userChanged.UserName != user.UserName)
            {
                user.UserName = userChanged.UserName;
            }
            if (userChanged.Password != user.Password)
            {
                user.Password = userChanged.Password;
            }
            if (db.Users.Where(u => u.Email == userChanged.Email).ToList().Count() == 0)
            {
                user.Email = userChanged.Email;
            }
            if (user.IsSysAdmin != userChanged.IsSysAdmin)
            {
                user.IsSysAdmin = userChanged.IsSysAdmin;
            }
            return (new Result { Status = HttpStatusCode.OK, Message = "Saved rows " + db.SaveChanges() }).ToString();
        }

        // DELETE: user/delete/5
        [HttpDelete("delete/{id}")]
        public string DeleteUser(Guid? id)
        {
            if (id == null)
            {
                return (new Result { Status = HttpStatusCode.NotFound, Message = "Id Not found" }).ToString();
            }
            var user = db.Users.FirstOrDefault(u => u.Id == id);
            if (user == null)
            {
                return (new Result { Status = HttpStatusCode.NotFound, Message = "User Not found" }).ToString();
            }
            db.Users.Remove(user);
            return (new Result { Status = HttpStatusCode.OK, Message = "Saved rows " + db.SaveChanges() }).ToString();
        }

        [HttpPost("login")]
        public string Login([FromBody][Bind("Email, Password")] UserLogin user)
        {
            var identity = GetIdentity(user.Email, user.Password);
            if (identity == null)
            {
                return (new Result { Status = HttpStatusCode.BadRequest, Message = "Incorrect Email or Password" }).ToString();
            }

            var now = DateTime.UtcNow;
            // создаем JWT-токен
            var jwt = new JwtSecurityToken(
                    issuer: AuthOptions.ISSUER,
                    audience: AuthOptions.AUDIENCE,
                    notBefore: now,
                    claims: identity.Claims,
                    expires: now.Add(TimeSpan.FromMinutes(AuthOptions.LIFETIME)),
                    signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));
            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

            var response = new
            {
                access_token = encodedJwt,
                email = identity.Name
            };

            return JsonConvert.SerializeObject(response);
        }

        private ClaimsIdentity GetIdentity(string email, string password)
        {
            var user = db.Users.FirstOrDefault(u => u.Email == email && u.Password == HeshMD5(password));
            string role = "User";
            if (user != null)
            {
                if (user.IsSysAdmin)
                {
                    role = "SysAdmin";
                }
                var claims = new List<Claim>
                {
                    new Claim(ClaimsIdentity.DefaultNameClaimType, user.Email),
                    new Claim(ClaimsIdentity.DefaultRoleClaimType, role)
                };
                ClaimsIdentity claimsIdentity =
                new ClaimsIdentity(claims, "Token", ClaimsIdentity.DefaultNameClaimType,
                    ClaimsIdentity.DefaultRoleClaimType);
                return claimsIdentity;
            }

            return null;

        }

        private string HeshMD5(string password)
        {
            byte[] bytes = Encoding.Unicode.GetBytes(password);
            MD5CryptoServiceProvider CSP = new MD5CryptoServiceProvider();
            byte[] byteHash = CSP.ComputeHash(bytes);
            string hash = string.Empty;

            foreach (byte b in byteHash)
                hash += string.Format("{0:x2}", b);

            return hash;
        }
    }
}