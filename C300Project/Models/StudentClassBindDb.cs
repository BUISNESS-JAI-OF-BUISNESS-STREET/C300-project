using System;
using System.Collections.Generic;

#nullable disable

namespace fyp.Models
{
    public partial class StudentClassBindDb
    {
        public int Id { get; set; }
        public int StudentId { get; set; }
        public int ClassId { get; set; }

        public virtual Class Class { get; set; }
        public virtual Student Student { get; set; }
    }
}
