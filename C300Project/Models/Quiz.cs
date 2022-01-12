using System;
using System.Collections.Generic;

#nullable disable

namespace fyp.Models
{
    public partial class Quiz
    {
        public Quiz()
        {
            QuizQuestionBindDb = new HashSet<QuizQuestionBindDb>();
        }

        public int QuizId { get; set; }
        public string Title { get; set; }
        public string Topic { get; set; }
        public int Sec { get; set; }
        public DateTime StartDt { get; set; }
        public DateTime EndDt { get; set; }
        public string UserCode { get; set; }

        public virtual Account UserCodeNavigation { get; set; }
        public virtual ICollection<QuizQuestionBindDb> QuizQuestionBindDb { get; set; }
    }
}
