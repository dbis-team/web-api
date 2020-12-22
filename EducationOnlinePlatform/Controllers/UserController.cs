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
using Microsoft.AspNetCore.Authorization;
using System.Net.Mail;
using MimeKit;
using Microsoft.AspNetCore.Identity;
using EducationOnlinePlatform.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.IO;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Net.Http.Headers;
using Microsoft.Extensions.Logging;
/*using System.Net.Mail;
using MimeKit;*/

namespace EducationOnlinePlatform.Controllers
{
    [EnableCors("CorsApi")]
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        ApplicationContext db;

        private readonly UserManager<User> _userManager;

        private readonly ILogger _logger;

        public UserController(UserManager<User> userManager, ApplicationContext context, ILogger<UserController> logger)
        {
            db = context;
            _userManager = userManager;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            _logger.LogInformation("Processing request {0}", Request.Path);
            var users = await db.Users.ToListAsync();
            return Ok(JsonConvert.SerializeObject(users));
        }

        // GET: User/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser(Guid? id)
        {
            _logger.LogInformation("Processing request {0}", Request.Path);
            var user = await db.Users.Where(u => u.Id == id).ToListAsync();
            if (user == null)
            {
                return NotFound();
            }
            return Ok(JsonConvert.SerializeObject(user));
        }
        // GET: User/me
        [HttpGet("me")]
        public async Task<IActionResult> GetCurrentUser()
        {
            _logger.LogInformation("Processing request {0}", Request.Path);
            var token = Request.Headers[HeaderNames.Authorization].ToString();
            token = token.Substring(7);
            try {
                var tokenHandler = new JwtSecurityTokenHandler();
                var securityToken = tokenHandler.ReadToken(token) as JwtSecurityToken;
                var email = securityToken.Claims.First(claim => claim.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name").Value;
                var user = await db.Users.Where(u => u.Email == email).ToListAsync();
                if (user == null)
                {
                    return NotFound();
                }
                return Ok(JsonConvert.SerializeObject(user));
            }
            catch
            {
                return NotFound("Bad Token");
            }
        }

        // POST: User/register
        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> RegisterUser([FromBody][Bind("UserName,Password,Email")] RegisterViewModel RegisterUser)
        {
            _logger.LogInformation("Processing request {0}", Request.Path);
            if (ModelState.IsValid)
            {
                if (await _userManager.FindByEmailAsync(RegisterUser.Email) == null)
                {
                    var user = new User { Email = RegisterUser.Email, UserName = RegisterUser.UserName, Role = RegisterUser.Role};
                    var result = await _userManager.CreateAsync(user, RegisterUser.Password);
                    if (result.Succeeded)
                    {
                        
                        EmailService emailService = new EmailService();
                        var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                        var builder = new ConfigurationBuilder();
                        builder.SetBasePath(Directory.GetCurrentDirectory());
                        builder.AddJsonFile("appsettings.json");
                        var config = builder.Build();
                        var utl = config.GetSection("clientAppEmailVerificationLink").Value;
                        var callbackUrl = $"{utl}?userId={user.Id}&code={code}";
                        await emailService.SendEmailAsync(RegisterUser.Email, "Confirm your account",
                            $"Подтвердите регистрацию, перейдя по ссылке: <a href='{callbackUrl}'>{callbackUrl}</a>");
                        return Ok(new Result { Status = HttpStatusCode.OK, Message = "Success registration" }.ToString());
                    }
                    return BadRequest(new Result { Status = HttpStatusCode.BadRequest, Message = result.ToString() }.ToString());
                }
                else
                {
                    return Conflict(new Result { Status = HttpStatusCode.Conflict, Message = "Email is busy" }.ToString());
                }
            }
            else
            {
                return BadRequest(new Result { Status = HttpStatusCode.BadRequest, Message = "Model is no correct" }.ToString());
            }
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("ConfirmEmail")]
        public async Task<IActionResult> ConfirmEmail(string userId, string code)
        {
            _logger.LogInformation("Processing request {0}", Request.Path);
            if (userId == null || code == null)
            {
                return NotFound("Not found email or code");
            }
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound("Not found email or code");
            }
            var result = await _userManager.ConfirmEmailAsync(user, code);
            if (result.Succeeded)
                return Ok("Succes confirm");
            else
                return NotFound("Not found email or code");
        }

        // PUT: /User/update/5
        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateUser(Guid? id, [FromBody] UpdateUserViewModel userUpdate )
        {
            _logger.LogInformation("Processing request {0}", Request.Path);
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null)
            {
                return NotFound(new Result { Status = HttpStatusCode.NotFound, Message = "User Not found" }.ToString());
            }

            if (userUpdate.UserName != user.UserName && userUpdate.UserName != null)
            {
                user.UserName = userUpdate.UserName;
            }
            if (userUpdate.Email != user.Email && userUpdate.Email != null)
            {
                user.Email = userUpdate.Email;
            }
            if (userUpdate.Password != user.PasswordHash && userUpdate.Password != null)
            {
                var _passwordValidator =
                     HttpContext.RequestServices.GetService(typeof(IPasswordValidator<User>)) as IPasswordValidator<User>;
                var _passwordHasher =
                     HttpContext.RequestServices.GetService(typeof(IPasswordHasher<User>)) as IPasswordHasher<User>;
                var result = await _passwordValidator.ValidateAsync(_userManager, user, userUpdate.Password);
                if (result.Succeeded)
                {
                    user.PasswordHash = _passwordHasher.HashPassword(user, userUpdate.Password);
                }
                else
                {
                    return BadRequest(result.Errors);
                }

            }
            await _userManager.UpdateAsync(user);
            return Ok("UpdateSuccess");
        }

        // DELETE: user/delete/5
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteUser(Guid? id)
        {
            _logger.LogInformation("Processing request {0}", Request.Path);
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null)
            {
                return NotFound(new Result { Status = HttpStatusCode.NotFound, Message = "User Not found" }.ToString());
            }
            var result = await _userManager.DeleteAsync(user);
            return Ok(new Result { Status = HttpStatusCode.OK, Message = "Success " + result.Succeeded }.ToString());
        }

        [HttpPost("login")]
        public async Task<IActionResult>Login([FromBody][Bind("Email, Password, RemeberMe")] UserLogin userlog)
        {
            _logger.LogInformation("Processing request {0}", Request.Path);
            var user = await _userManager.FindByEmailAsync(userlog.Email);
            var result = await _userManager.CheckPasswordAsync(user, userlog.Password);
            if (result)
            {
               if (!await _userManager.IsEmailConfirmedAsync(user))
                {
                    return BadRequest(new Result { Status = HttpStatusCode.BadRequest, Message = "Вы не подтверлдили свой email" }.ToString());
                }
                var role = user.Role.ToString();
                if (role == null)
                {
                    role = "Student";
                }
                var identity = GetIdentity(userlog.Email, role);
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
                //await _userManager.AddClaimsAsync(user, identity.Claims.ToArray());
                return Ok(JsonConvert.SerializeObject(response));
            }
            return BadRequest(new Result { Status = HttpStatusCode.BadRequest, Message = "Incorrect Email or Password" }.ToString());
        }
        // POST: /ChangeRole
        [HttpPost("ChangeRole")]
        public async Task<IActionResult> ChangeRole([FromBody] ChangeRoleViewModel ChangeRole)
        {
            _logger.LogInformation("Processing request {0}", Request.Path);
            var user = await _userManager.FindByEmailAsync(ChangeRole.Email);

            if (user == null)
            {
                return NotFound(new Result { Status = HttpStatusCode.NotFound, Message = "User not found" }.ToString());
            }

            if (ChangeRole.Role == user.Role)
            {
                return BadRequest(new Result { Status = HttpStatusCode.BadRequest, Message = "Role not changed" }.ToString());
            }

            user.Role = ChangeRole.Role;
            await _userManager.UpdateAsync(user);

            return Ok(new Result { Status = HttpStatusCode.OK, Message = "Role has changed!" }.ToString());
        }
        private ClaimsIdentity GetIdentity(string email, string role)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, email),
                new Claim(ClaimsIdentity.DefaultRoleClaimType, role)
            };
            ClaimsIdentity claimsIdentity =
            new ClaimsIdentity(claims, "Token", ClaimsIdentity.DefaultNameClaimType,
                ClaimsIdentity.DefaultRoleClaimType);
            return claimsIdentity;
        }

    }
}