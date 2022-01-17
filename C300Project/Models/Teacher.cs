using System;
using System.Collections.Generic;

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
        public string Name { get; set; }
        public string MobileNo { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public string AddedBy { get; set; }

        public virtual Account AddedByNavigation { get; set; }
        public virtual ICollection<TeacherClassBindDb> TeacherClassBindDb { get; set; }
        public virtual ICollection<TeacherStudentBindDb> TeacherStudentBindDb { get; set; }
    }
}
