using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EducationOnlinePlatform.Models
{
    //Описывает в какие эдукейшин сеты добавлены юзеры и какие у них в нем роли(преподоваитель может быть студентом в другом эдукейшин сете)
    public class UserInEducationSet
    {
        [Key]
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public User User { get; set; }
        public Guid EducationSetId { get; set; }
        public EducationSet EducationSet { get; set; }
    }
}
