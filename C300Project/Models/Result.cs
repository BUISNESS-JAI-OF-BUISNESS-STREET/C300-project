using System;
using System.Collections.Generic;

#nullable disable

namespace fyp.Models
{
    public partial class Result
    {
        public int Id { get; set; }
        public int QuizId { get; set; }
        public string StudentId { get; set; }
        public string StudentName { get; set; }
        public string Topic { get; set; }
        public int Score { get; set; }
        public bool Attempt { get; set; }
        public DateTime ExamDate { get; set; }
    }
}
