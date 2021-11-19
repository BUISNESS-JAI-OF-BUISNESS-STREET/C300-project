using System;
using System.Collections.Generic;

#nullable disable

namespace fyp.Models
{
    public partial class MugOrder
    {
        public int Id { get; set; }
        public int Qty { get; set; }
        public string Color { get; set; }
        public int PokedexId { get; set; }
        public string CreatedBy { get; set; }
    }
}
