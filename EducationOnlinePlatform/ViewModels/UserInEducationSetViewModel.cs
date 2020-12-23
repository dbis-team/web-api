using System;
using System.ComponentModel.DataAnnotations;

namespace EducationOnlinePlatform.ViewModels
{
    public class UserInEducationSetViewModel
    {
        [Required]
        public Guid UserId { get; set; }
        [Required]
        public Guid EducationSetId { get; set; }

    }
}
