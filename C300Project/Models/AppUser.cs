using System;
using System.Collections.Generic;

#nullable disable

namespace fyp.Models
{
    public partial class AppUser
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public byte[] Password { get; set; }
        public string Role { get; set; }
    }
}
