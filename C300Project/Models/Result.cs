using System;
using System.Collections.Generic;

#nullable disable

namespace fyp.Models
{
    public partial class Result
    {
        public int ResultId { get; set; }
        public int QuizId { get; set; }
        public string AccountId { get; set; }
        public string Name { get; set; }
        public string Title { get; set; }
        public string Topic { get; set; }
        public int Score { get; set; }
        public bool Attempt { get; set; }
        public DateTime Dt { get; set; }
    }
}
