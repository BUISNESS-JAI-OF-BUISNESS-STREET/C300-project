using System;
using System.Collections.Generic;

#nullable disable

namespace fyp.Models
{
    public partial class Course
    {

        public int Id { get; set; }
        public string Level { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Code { get; set; }
        public string Duration { get; set; }
        public string School { get; set; }

    }
}
