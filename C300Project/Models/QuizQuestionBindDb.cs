using System;
using System.Collections.Generic;

#nullable disable

namespace fyp.Models
{
    public partial class QuizQuestionBindDb
    {
        public int Id { get; set; }
        public int QuestionId { get; set; }
        public int QuizId { get; set; }

        public virtual Question Question { get; set; }
        public virtual Quiz Quiz { get; set; }
    }
}
