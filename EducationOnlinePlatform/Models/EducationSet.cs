using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EducationOnlinePlatform.Models
{
    public class EducationSet
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        public string Name { get; set; }
        public List<Subject> Subjects { get; set; }
        public List<UserInEducationSet> UserInEducationSet { get; set; }

        public EducationSet()
        {
            UserInEducationSet = new List<UserInEducationSet>();
        }
    }
}
