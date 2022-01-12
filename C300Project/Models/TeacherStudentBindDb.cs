using System;
using System.Collections.Generic;

#nullable disable

namespace fyp.Models
{
    public partial class TeacherStudentBindDb
    {
        public int Id { get; set; }
        public int StudentId { get; set; }
        public int TeacherId { get; set; }

        public virtual Student Student { get; set; }
        public virtual Teacher Teacher { get; set; }
    }
}
