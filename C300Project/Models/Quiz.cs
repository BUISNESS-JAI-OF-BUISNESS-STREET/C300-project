using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

#nullable disable

namespace fyp.Models
{
    public partial class Quiz
    {
        public Quiz()
        {
            QuizClassBindDb = new HashSet<QuizClassBindDb>();
            QuizQuestionBindDb = new HashSet<QuizQuestionBindDb>();
        }

        public int QuizId { get; set; }
        [Required(ErrorMessage = "Title Required")]
        public string Title { get; set; }
        [Required(ErrorMessage = "Topic Required")]
        public int Topic { get; set; }
        [Required(ErrorMessage = "Level Required")]
        public int Sec { get; set; }
        [Required(ErrorMessage = "Start Date Required")]
        public DateTime StartDt { get; set; }
        [Required(ErrorMessage = "End Date Required")]
        public DateTime EndDt { get; set; }
        public int Duration { get; set; }
        public string UserCode { get; set; }

        public virtual Topic TopicNavigation { get; set; }
        public virtual Account UserCodeNavigation { get; set; }
        public virtual ICollection<QuizClassBindDb> QuizClassBindDb { get; set; }
        public virtual ICollection<QuizQuestionBindDb> QuizQuestionBindDb { get; set; }
    }
}
