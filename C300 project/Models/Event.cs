using System;
using System.Collections.Generic;

#nullable disable

namespace C300_project.Models
{
    public partial class Event
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string Description { get; set; }
        public DateTime Date { get; set; }
        public string Time { get; set; }
    }
}
