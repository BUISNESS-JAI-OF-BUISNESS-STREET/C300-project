using System;
using System.Collections.Generic;

#nullable disable

namespace fyp.Models
{
    public partial class TeacherClassBindDb
    {
        public int Id { get; set; }
        public int TeacherId { get; set; }
        public int ClassId { get; set; }

        public virtual Class Class { get; set; }
        public virtual Teacher Teacher { get; set; }
    }
}
