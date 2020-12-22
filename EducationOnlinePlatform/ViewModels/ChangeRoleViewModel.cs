using EducationOnlinePlatform.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EducationOnlinePlatform.ViewModels
{
    public class ChangeRoleViewModel
    {
        [Required]
        public string Email { get; set; }
        [Required]
        public Role Role { get; set; }
    }
}
