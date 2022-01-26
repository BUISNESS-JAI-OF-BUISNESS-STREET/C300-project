using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
#nullable disable

namespace fyp.Models
{
    public partial class Announcement
    {
        public int Id { get; set; }
        public int ClassId { get; set; }
        public string Remarks { get; set; }

        public virtual Class Class { get; set; }
    }
}
