using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EducationOnlinePlatform.Models
{
    public class User
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        public string UserName { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public string Email { get; set; }

        public bool IsSysAdmin { get; set; }

        public List<UserInEducationSet> UserInEducationSet { get; set; }

        public User() 
        {
            UserInEducationSet = new List<UserInEducationSet>();
        }
    }
}
