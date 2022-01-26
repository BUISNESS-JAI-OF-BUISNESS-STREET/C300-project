using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
#nullable disable

namespace fyp.Models
{
    public partial class Teacher
    {
        public Teacher()
        {
            TeacherClassBindDb = new HashSet<TeacherClassBindDb>();
            TeacherStudentBindDb = new HashSet<TeacherStudentBindDb>();
        }

        public int TeacherId { get; set; }

        [Required(ErrorMessage = "Name must not be empty!")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Phone Number must not be empty!")]
        public string MobileNo { get; set; }

        [Required(ErrorMessage = "Email must not be empty!")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Role must not be empty!")]
        public string Role { get; set; }
        public string AddedBy { get; set; }

        public virtual Account AddedByNavigation { get; set; }
        public virtual ICollection<TeacherClassBindDb> TeacherClassBindDb { get; set; }
        public virtual ICollection<TeacherStudentBindDb> TeacherStudentBindDb { get; set; }
    }
}
