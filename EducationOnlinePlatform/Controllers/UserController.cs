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
/*using System.Net.Mail;
using MimeKit;*/

namespace EducationOnlinePlatform.Controllers
{
    [EnableCors("CorsApi")]
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
        public IActionResult GetUser(Guid? id)
        {
            if (id == null)
            {
                return NotFound((new Result { Status = HttpStatusCode.NotFound, Message = "Id Not found" }).ToString());
            }
            var user = db.Users.FirstOrDefault(u => u.Id == id);
            if (user == null)
            {
                return NotFound((new Result { Status = HttpStatusCode.NotFound, Message = "User Not found" }).ToString());
            }
            return Ok(System.Text.Json.JsonSerializer.Serialize<User>(user));
        }

        // POST: User/register
        [HttpPost]
        [Route("register")]
        public IActionResult RegisterUser([FromBody][Bind("UserName,Password,Email,IsSysAdmin")] RegisterViewModel RegisterUser)
        {
            if (ModelState.IsValid)
            {
                var users = db.Users;
                if (users.FirstOrDefault(u => u.Email == RegisterUser.Email) == null)
                {
                    var user = new User { Password = HeshMD5(RegisterUser.Password), Email = RegisterUser.Email, UserName = RegisterUser.Email, IsSysAdmin = RegisterUser.IsSysAdmin};
                    users.Add(user);

                    //EmailCofirm(user);
                    return Ok(new Result { Status = HttpStatusCode.OK, Message = "Saved Rows " + db.SaveChanges() }.ToString());
                }
                else
                {
                    return Conflict(new Result { Status = HttpStatusCode.Conflict, Message = "Email is busy" }.ToString());
                }
            }
            else
            {
                return BadRequest(new Result { Status = HttpStatusCode.BadRequest, Message = "Email is busy" }.ToString());
            }
        }

        /*private async void EmailCofirm(string email, string subject, string message)
        {
            var emailMessage = new MimeMessage();

            emailMessage.From.Add(new MailboxAddress("Администрация сайта", "login@yandex.ru"));
            emailMessage.To.Add(new MailboxAddress("", email));
            emailMessage.Subject = subject;
            emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html)
            {
                Text = message
            };

            SmtpClient client = new SmtpClient("smtp.yandex.ru", 25);
            SmtpClient smtp = new SmtpClient("smtp.yandex.ru", 25);
            // логин и пароль
            smtp.Credentials = new NetworkCredential("somemail@yandex.ru", "password");
            //await smtp.Send(emailMessage);
            using (var client = new SmtpClient())
            {
                client.SendMailAsync
                await client.ConnectAsync("smtp.yandex.ru", 25, false);
                await client.AuthenticateAsync("login@yandex.ru", "password");
                await client.SendAsync(emailMessage);

                await client.DisconnectAsync(true);
            }
        }*/

        // PUT: /User/update/5
        [HttpPut("update/{id}")]
        public IActionResult UpdateUser(Guid? id, [FromBody] UpdateUserViewModel userUpdate )
        {
            if (id == null)
            {
                return NotFound(new Result { Status = HttpStatusCode.NotFound, Message = "Id Not found" }.ToString());
            }

            var user = db.Users.FirstOrDefault(u => u.Id == id);
            if (user == null)
            {
                return NotFound(new Result { Status = HttpStatusCode.NotFound, Message = "User Not found" }.ToString());
            }
            if (userUpdate.UserName != user.UserName)
            {
                user.UserName = userUpdate.UserName;
            }
            if (userUpdate.Password != user.Password)
            {
                user.Password = userUpdate.Password;
            }
            if (db.Users.Where(u => u.Email == userUpdate.Email).ToList().Count() == 0)
            {
                user.Email = userUpdate.Email;
            }
            if (user.IsSysAdmin != userUpdate.IsSysAdmin)
            {
                user.IsSysAdmin = userUpdate.IsSysAdmin;
            }
            return Ok(new Result { Status = HttpStatusCode.OK, Message = "Saved rows " + db.SaveChanges() }.ToString());
        }

        // DELETE: user/delete/5
        [HttpDelete("delete/{id}")]
        public IActionResult DeleteUser(Guid? id)
        {
            if (id == null)
            {
                return NotFound(new Result { Status = HttpStatusCode.NotFound, Message = "Id Not found" }.ToString());
            }
            var user = db.Users.FirstOrDefault(u => u.Id == id);
            if (user == null)
            {
                return NotFound(new Result { Status = HttpStatusCode.NotFound, Message = "User Not found" }.ToString());
            }
            db.Users.Remove(user);
            return Ok(new Result { Status = HttpStatusCode.OK, Message = "Saved rows " + db.SaveChanges() }.ToString());
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody][Bind("Email, Password")] UserLogin user)
        {
            var identity = GetIdentity(user.Email, user.Password);
            if (identity == null)
            {
                return BadRequest(new Result { Status = HttpStatusCode.BadRequest, Message = "Incorrect Email or Password" }.ToString());
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

            return Ok(JsonConvert.SerializeObject(response));
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