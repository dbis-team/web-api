using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EducationOnlinePlatform.ViewModels
{
    public class AddSubjectViewModel
    {
        [Required]
        public string Name { get; set; }
        public string Description { get; set; }
        [Required]
        public Guid EducationSetId { get; set; }
    }
}
