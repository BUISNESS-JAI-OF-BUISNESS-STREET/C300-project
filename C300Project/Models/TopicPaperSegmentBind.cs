using System;
using System.Collections.Generic;

#nullable disable

namespace fyp.Models
{
    public partial class TopicPaperSegmentBind
    {
        public int TopicId { get; set; }
        public int PapersId { get; set; }
        public int SegmentId { get; set; }

        public virtual Papers Papers { get; set; }
        public virtual Segment Segment { get; set; }
        public virtual Topic Topic { get; set; }
    }
}
