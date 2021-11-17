using System;
using System.Collections.Generic;

#nullable disable

namespace C300_project.Models
{
    public partial class Reports
    {
        public int Id { get; set; }
        public int QuizId { get; set; }
        public string StudentId { get; set; }
        public string StudentName { get; set; }
        public string Topic { get; set; }
        public int Score { get; set; }
        public bool Attempt { get; set; }
    }
}
