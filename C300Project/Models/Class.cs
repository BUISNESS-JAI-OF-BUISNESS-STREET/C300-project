using System;
using System.Collections.Generic;

#nullable disable

namespace fyp.Models
{
    public partial class Class
    {
        public Class()
        {
            Announcement = new HashSet<Announcement>();
            StudentClassBindDb = new HashSet<StudentClassBindDb>();
            TeacherClassBindDb = new HashSet<TeacherClassBindDb>();
        }

        public int ClassId { get; set; }
        public string Name { get; set; }

        public virtual ICollection<Announcement> Announcement { get; set; }
        public virtual ICollection<StudentClassBindDb> StudentClassBindDb { get; set; }
        public virtual ICollection<TeacherClassBindDb> TeacherClassBindDb { get; set; }
    }
}
