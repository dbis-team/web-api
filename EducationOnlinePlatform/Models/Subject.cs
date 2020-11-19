using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EducationOnlinePlatform.Models
{
    public class Subject
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        public string Name { get; set; }

        public Guid EducationSetId { get; set; }
        public EducationSet EducationSet { get; set; }
    }
}
