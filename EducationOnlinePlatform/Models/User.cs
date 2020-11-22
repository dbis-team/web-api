using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EducationOnlinePlatform.Models
{
    public class User
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        public string UserName { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [JsonIgnore]
        public string Password { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        public bool IsSysAdmin { get; set; }

        public List<UserInEducationSet> UserInEducationSet { get; set; }

        public User() 
        {
            UserInEducationSet = new List<UserInEducationSet>();
        }
    }
}
