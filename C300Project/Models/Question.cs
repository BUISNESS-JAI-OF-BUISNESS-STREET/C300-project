using System;
using System.Collections.Generic;

#nullable disable

namespace fyp.Models
{
    public partial class Question
    {
        public int QuestionId { get; set; }
        public int QuizId { get; set; }
        public string Questions { get; set; }
        public string FirstOption { get; set; }
        public string SecondOption { get; set; }
        public string ThirdOption { get; set; }
        public string FourthOption { get; set; }
        public string Topic { get; set; }
        public string CorrectAns { get; set; }
        public string UserCode { get; set; }

        public virtual Account UserCodeNavigation { get; set; }
    }
}
