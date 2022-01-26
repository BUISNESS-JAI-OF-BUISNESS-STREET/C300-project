using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
#nullable disable

namespace fyp.Models
{
    public partial class Announcement
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Announcement must be assigned to a class!")]
        public int ClassId { get; set; }

        [Required(ErrorMessage = "Announcement must not be empty!")]
        public string Remarks { get; set; }

        public virtual Class Class { get; set; }
    }
}
