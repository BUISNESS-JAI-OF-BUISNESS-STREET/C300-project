using System;
using System.Collections.Generic;

#nullable disable

namespace C300_project.Models
{
    public partial class Student
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int CourseId { get; set; }
        public string Email { get; set; }
        public int ContactNum { get; set; }
        public DateTime EnrolDate { get; set; }
        public int Year { get; set; }

        public virtual Course Course { get; set; }
    }
}
