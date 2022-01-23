using System;
using System.Collections.Generic;

#nullable disable

namespace fyp.Models
{
    public partial class QuizClassBindDb
    {
        public int Id { get; set; }
        public int QuizId { get; set; }
        public int ClassId { get; set; }

        public virtual Class Class { get; set; }
        public virtual Quiz Quiz { get; set; }
    }
}
