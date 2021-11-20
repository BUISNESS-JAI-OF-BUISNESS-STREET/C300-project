using System;
using System.Collections.Generic;

#nullable disable

namespace fyp.Models
{
    public partial class Quiz
    {
        public int QuizId { get; set; }
        public string Title { get; set; }
        public string Topic { get; set; }
        public int Sec { get; set; }
        public DateTime Dt { get; set; }
    }
}
