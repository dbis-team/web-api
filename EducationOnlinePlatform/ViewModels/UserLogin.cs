using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace EducationOnlinePlatform.ViewModels
{
    public class UserLogin
    {
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [DefaultValue(false)]
        public bool RememberMe { get; set; }
    }
}
