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
        private readonly SignInManager<User> _signInManager;

        public UserController(UserManager<User> userManager, SignInManager<User> signInManager, ApplicationContext context)
        {
            db = context;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            var users = await( from u in db.Users
                        join ur in db.UserRoles on u.Id equals ur.UserId
                        join r in db.Roles on ur.RoleId equals r.Id
                        select new
                        {
                            id = u.Id,
                            userName = u.UserName,
                            email = u.Email,
                            phone = u.PhoneNumber,
                            roleId = r.Id,
                            role = r.Name
                        }).ToListAsync();
            return Ok(JsonConvert.SerializeObject(users));
        }

        // GET: User/5
        [HttpGet("{id}")]
        public IActionResult GetUser(Guid? id)
        {
            var user = (from u in db.Users
                        join ur in db.UserRoles on u.Id equals ur.UserId
                        join r in db.Roles on ur.RoleId equals r.Id
                        where u.Id == id
                        select new
                        {
                            userName = u.UserName,
                            email = u.Email,
                            phone = u.PhoneNumber,
                            roleId = r.Id,
                            role = r.Name
                        });
            if (user == null)
            {
                return NotFound();
            }
            return Ok(JsonConvert.SerializeObject(user));
        }
        // GET: User/me
        [HttpGet("me")]
        public IActionResult GetCurrentUser()
        {
            var user = (from u in db.Users
                               join ur in db.UserRoles on u.Id equals ur.UserId
                               join r in db.Roles on ur.RoleId equals r.Id
                               where u.Email == User.Identity.Name
                               select new
                               {
                                   id = u.Id,
                                   userName = u.UserName,
                                   email = u.Email,
                                   phone = u.PhoneNumber,
                                   roleId = r.Id,
                                   role = r.Name
                               });
            if(user == null)
            {
                return NotFound();
            }
            return Ok(JsonConvert.SerializeObject(user));
        }

        // POST: User/register
        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> RegisterUser([FromBody][Bind("UserName,Password,Email")] RegisterViewModel RegisterUser)
        {
            if (ModelState.IsValid)
            {
                if (await _userManager.FindByEmailAsync(RegisterUser.Email) == null)
                {
                    var user = new User { Email = RegisterUser.Email, UserName = RegisterUser.UserName};
                    var result = await _userManager.CreateAsync(user, RegisterUser.Password);
                    if (result.Succeeded)
                    {
                        await _userManager.AddToRoleAsync(user, RegisterUser.Role);
                        EmailService emailService = new EmailService();
                        var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                        var callbackUrl = Url.Action(
                            "ConfirmEmail",
                            "User",
                            new { userId = (user.Id).ToString(), code = code },
                            protocol: HttpContext.Request.Scheme);
                        await emailService.SendEmailAsync(RegisterUser.Email, "Confirm your account",
                            $"Подтвердите регистрацию, перейдя по ссылке: <a href='{callbackUrl}'>{callbackUrl}'</a>");
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
            var user = await _userManager.FindByEmailAsync(userlog.Email);
            var result = await _userManager.CheckPasswordAsync(user, userlog.Password);
            if (result)
            {
                if (!await _userManager.IsEmailConfirmedAsync(user))
                {
                    return BadRequest(new Result { Status = HttpStatusCode.BadRequest, Message = "Вы не подтверлдили свой email" }.ToString());
                }
                //Must be one role ??
                var role = (await _userManager.GetRolesAsync(user)).FirstOrDefault();
                if(role == null)
                {
                    role = "student";
                }
                var claims = new List<Claim>
                {
                    new Claim(ClaimsIdentity.DefaultNameClaimType, user.Email),
                    new Claim(ClaimsIdentity.DefaultRoleClaimType, role)
                };
                var now = DateTime.UtcNow;
                // создаем JWT-токен
                var jwt = new JwtSecurityToken(
                        issuer: AuthOptions.ISSUER,
                        audience: AuthOptions.AUDIENCE,
                        notBefore: now,
                        claims: claims,
                        expires: now.Add(TimeSpan.FromMinutes(AuthOptions.LIFETIME)),
                        signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));
                var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

                var response = new
                {
                    access_token = encodedJwt,
                    email = user.Email
                };

                return Ok(JsonConvert.SerializeObject(response));
            }
            return BadRequest(new Result { Status = HttpStatusCode.BadRequest, Message = "Incorrect Email or Password" }.ToString());
        }
    }
}