using System;
using System.Collections.Generic;

#nullable disable

namespace fyp.Models
{
    public partial class Topic
    {
        public Topic()
        {
            Question = new HashSet<Question>();
            Quiz = new HashSet<Quiz>();
        }

        public int TopicId { get; set; }
        public string Name { get; set; }

        public virtual ICollection<Question> Question { get; set; }
        public virtual ICollection<Quiz> Quiz { get; set; }
    }
}
