using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EducationOnlinePlatform.Models
{
    public class Subject
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public Guid EducationSetId { get; set; }
        public string Description { get; set; }
        public EducationSet EducationSet { get; set; }
        public List<SubjectFile> File { get; set; } = new List<SubjectFile>();
    }
}
