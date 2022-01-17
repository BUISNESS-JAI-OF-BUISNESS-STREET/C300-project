using System;
using System.Collections.Generic;

#nullable disable

namespace fyp.Models
{
    public partial class Account
    {
        public Account()
        {
            Class = new HashSet<Class>();
            Question = new HashSet<Question>();
            Quiz = new HashSet<Quiz>();
            Student = new HashSet<Student>();
            Teacher = new HashSet<Teacher>();
        }

        public string AccountId { get; set; }
        public string Name { get; set; }
        public byte[] Password { get; set; }
        public string Role { get; set; }

        public virtual ICollection<Class> Class { get; set; }
        public virtual ICollection<Question> Question { get; set; }
        public virtual ICollection<Quiz> Quiz { get; set; }
        public virtual ICollection<Student> Student { get; set; }
        public virtual ICollection<Teacher> Teacher { get; set; }
    }
}
