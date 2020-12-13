using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace EducationOnlinePlatform.Models
{
    public class User: IdentityUser<Guid>
    {
        [DefaultValue(false)]

        [JsonIgnore]
        public override string PasswordHash { get; set; }
        [JsonIgnore]
        public override string SecurityStamp { get; set; }
        [JsonIgnore]
        public override string ConcurrencyStamp { get; set; }
        public Role Role { get; set; } = Role.Student;
        public List<UserInEducationSet> UserInEducationSet { get; set; }

        public User() 
        {
            UserInEducationSet = new List<UserInEducationSet>();
        }
    }
}
