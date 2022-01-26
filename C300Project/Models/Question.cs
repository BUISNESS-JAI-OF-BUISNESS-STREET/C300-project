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

        [Required(ErrorMessage = "Question must not be empty!")]
        public string Questions { get; set; }

        [Required(ErrorMessage = "Option must not be empty!")]
        public string FirstOption { get; set; }

        [Required(ErrorMessage = "Option must not be empty!")]
        public string SecondOption { get; set; }

        [Required(ErrorMessage = "Option must not be empty!")]
        public string ThirdOption { get; set; }

        [Required(ErrorMessage = "Option must not be empty!")]
        public string FourthOption { get; set; }

        [Required(ErrorMessage = "Topic must not be empty!")]
        public string Topic { get; set; }

        [Required(ErrorMessage = "There must be a correct answer!")]
        public string CorrectAns { get; set; }

        [Required(ErrorMessage = "Segment must not be empty!")]
        public string Segment { get; set; }
        public string UserCode { get; set; }

        public virtual Account UserCodeNavigation { get; set; }
        public virtual ICollection<QuizQuestionBindDb> QuizQuestionBindDb { get; set; }
    }
}
