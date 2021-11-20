using System;
using System.Collections.Generic;

#nullable disable

namespace fyp.Models
{
    public partial class Account
    {
        public Account()
        {
            Result = new HashSet<Result>();
        }

        public string AccountId { get; set; }
        public string Name { get; set; }
        public byte[] Password { get; set; }
        public string Role { get; set; }

        public virtual ICollection<Result> Result { get; set; }
    }
}
