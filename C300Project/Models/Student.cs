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
        public string Name { get; set; }
        public string MobileNo { get; set; }
        public string Country { get; set; }
        public bool Foreigner { get; set; }
        public string SchLvl { get; set; }
        public string Email { get; set; }
        public string Class { get; set; }
        public string AddedBy { get; set; }

        public virtual Account AddedByNavigation { get; set; }
        public virtual ICollection<StudentClassBindDb> StudentClassBindDb { get; set; }
        public virtual ICollection<TeacherStudentBindDb> TeacherStudentBindDb { get; set; }
    }
}
