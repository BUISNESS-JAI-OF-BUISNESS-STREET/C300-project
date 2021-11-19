using System;
using System.Collections.Generic;

#nullable disable

namespace fyp.Models
{
    public partial class ShirtOrder
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Qty { get; set; }
        public double Price { get; set; }
        public string Color { get; set; }
        public int PokedexId { get; set; }
        public int FrontPosition { get; set; }
        public string CreatedBy { get; set; }
    }
}
