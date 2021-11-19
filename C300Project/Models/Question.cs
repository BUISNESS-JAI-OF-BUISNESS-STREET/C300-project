using System;
using System.Collections.Generic;

#nullable disable

namespace fyp.Models
{
    public partial class Question
    {
        public int Id { get; set; }
        public string Question1 { get; set; }
        public string Ans1 { get; set; }
        public string Ans2 { get; set; }
        public string Ans3 { get; set; }
        public string Ans4 { get; set; }
        public string Topic { get; set; }
        public string CorrectAns { get; set; }
    }
}
