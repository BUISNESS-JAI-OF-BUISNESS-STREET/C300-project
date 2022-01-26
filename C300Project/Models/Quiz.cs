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

        [Required(ErrorMessage = "Title must not be empty!")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Topic must not be empty!")]
        public string Topic { get; set; }

        [Required(ErrorMessage = "School Level must not be empty!")]
        public int Sec { get; set; }

        [Required(ErrorMessage = "Start Date must not be empty!")]
        public DateTime StartDt { get; set; }

        [Required(ErrorMessage = "End Date must not be empty!")]
        public DateTime EndDt { get; set; }
        public int Duration { get; set; }
        public string UserCode { get; set; }

        public virtual Account UserCodeNavigation { get; set; }
        public virtual ICollection<QuizClassBindDb> QuizClassBindDb { get; set; }
        public virtual ICollection<QuizQuestionBindDb> QuizQuestionBindDb { get; set; }
    }
}
