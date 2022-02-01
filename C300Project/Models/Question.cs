using System;
using System.Collections.Generic;

#nullable disable

namespace fyp.Models
{
    public partial class Question
    {
        public Question()
        {
            QuizQuestionBindDb = new HashSet<QuizQuestionBindDb>();
        }

        public int QuestionId { get; set; }
        public string Questions { get; set; }
        public string FirstOption { get; set; }
        public string SecondOption { get; set; }
        public string ThirdOption { get; set; }
        public string FourthOption { get; set; }
        public int Topic { get; set; }
        public string CorrectAns { get; set; }
        public int? Segment { get; set; }
        public string UserCode { get; set; }

        public virtual Segment SegmentNavigation { get; set; }
        public virtual Topic TopicNavigation { get; set; }
        public virtual Account UserCodeNavigation { get; set; }
        public virtual ICollection<QuizQuestionBindDb> QuizQuestionBindDb { get; set; }
    }
}
