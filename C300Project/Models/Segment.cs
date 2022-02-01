using System;
using System.Collections.Generic;

#nullable disable

namespace fyp.Models
{
    public partial class Segment
    {
        public Segment()
        {
            Question = new HashSet<Question>();
        }

        public int SegmentId { get; set; }
        public string Name { get; set; }

        public virtual ICollection<Question> Question { get; set; }
    }
}
