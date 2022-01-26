using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
#nullable disable

namespace fyp.Models
{
    public partial class Student
    {
        public Student()
        {
            StudentClassBindDb = new HashSet<StudentClassBindDb>();
            TeacherStudentBindDb = new HashSet<TeacherStudentBindDb>();
        }

        public int StudentId { get; set; }

        [Required(ErrorMessage = "Name must not be empty!")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Phone Number must not be empty!")]
        public string MobileNo { get; set; }

        [Required(ErrorMessage = "Country must not be empty!")]
        public string Country { get; set; }
        public bool Foreigner { get; set; }

        [Required(ErrorMessage = "School Level must not be empty!")]
        public string SchLvl { get; set; }

        [Required(ErrorMessage = "Email must not be empty!")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Class must not be empty!")]
        public string Class { get; set; }
        public string AddedBy { get; set; }

        public virtual Account AddedByNavigation { get; set; }
        public virtual ICollection<StudentClassBindDb> StudentClassBindDb { get; set; }
        public virtual ICollection<TeacherStudentBindDb> TeacherStudentBindDb { get; set; }
    }
}
