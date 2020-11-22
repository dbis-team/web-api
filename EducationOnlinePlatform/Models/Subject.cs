using System;
using System.ComponentModel.DataAnnotations;

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
