using System;
using System.Collections.Generic;

#nullable disable

namespace fyp.Models
{
    public partial class Class
    {
        public Class()
        {
            Announcement = new HashSet<Announcement>();
        }

        public int ClassId { get; set; }
        public string ClassCode { get; set; }

        public virtual ICollection<Announcement> Announcement { get; set; }
    }
}
