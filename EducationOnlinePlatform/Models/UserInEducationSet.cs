using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EducationOnlinePlatform.Models
{
    public class UserInEducationSet
    {
        [Key]
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public User User { get; set; }
        public Guid EducationSetId { get; set; }
        public EducationSet EducationSet { get; set; }

        public Role UserRole { get; set; }

    }
}
