using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EducationOnlinePlatform.Models
{
    public class SubjectFile
    {
        public Guid Id {get; set;}
        public Guid SubjectId { get; set; }
        public Subject Subject { get; set; }
    }
}
