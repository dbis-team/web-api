using EducationOnlinePlatform.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EducationOnlinePlatform.Controllers
{
    public class ScheduleEvent
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        public string Name { get; set; }
        public string Description { get; set; }
        [Required]
        public DateTime DateTime { get; set;}
        [Required]
        public Guid EducationSetId { get; set; }
        public EducationSet EducationSet { get; set; }
    }
}
