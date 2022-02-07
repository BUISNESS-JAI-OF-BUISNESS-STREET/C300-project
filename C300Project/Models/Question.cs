using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

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
        [Required(ErrorMessage = "Question Required")]
        public string Questions { get; set; }
        [Required(ErrorMessage = "Option Required")]
        public string FirstOption { get; set; }
        [Required(ErrorMessage = "Option Required")]
        public string SecondOption { get; set; }
        [Required(ErrorMessage = "Option Required")]
        public string ThirdOption { get; set; }
        [Required(ErrorMessage = "Option Required")]
        public string FourthOption { get; set; }
        [Required(ErrorMessage = "Topic Required")]
        public int Topic { get; set; }
        [Required(ErrorMessage = "Correct Answer Required")]
        public string CorrectAns { get; set;}
        [Required(ErrorMessage = "Segment Required")]
        public int? Segment { get; set; }
        public string UserCode { get; set; }

        public virtual Segment SegmentNavigation { get; set; }
        public virtual Topic TopicNavigation { get; set; }
        public virtual Account UserCodeNavigation { get; set; }
        public virtual ICollection<QuizQuestionBindDb> QuizQuestionBindDb { get; set; }
    }
}
