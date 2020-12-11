using EducationOnlinePlatform.Models;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace EducationOnlinePlatform.Data
{
    public static class RoleInitializer
    {
        public static async Task InitializeAsync(RoleManager<ApplicationRole> roleManager)
        {
            if (await roleManager.FindByNameAsync("admin") == null)
            {
                await roleManager.CreateAsync(new ApplicationRole { Name = "admin" });
            }
            if (await roleManager.FindByNameAsync("student") == null)
            {
                await roleManager.CreateAsync(new ApplicationRole { Name = "student" });
            }
            if (await roleManager.FindByNameAsync("teacher") == null)
            {
                await roleManager.CreateAsync(new ApplicationRole { Name = "teacher" });
            }
        }
    }
}
