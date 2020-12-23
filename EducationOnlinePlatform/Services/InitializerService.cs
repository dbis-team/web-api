using EducationOnlinePlatform.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EducationOnlinePlatform.Services
{
    public class InitializerService
    {
        public static async Task InitializeAsync(UserManager<User> userManager)
        {
            string adminEmail = "educationonlineplatformkm73@gmail.com";
            string password = "000000";
            if (await userManager.FindByNameAsync(adminEmail) == null)
            {
                var admin = new User { Email = adminEmail, UserName = adminEmail, Role = 0};
                await userManager.CreateAsync(admin, password);
            }
        }
    }
}
