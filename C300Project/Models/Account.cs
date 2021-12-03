using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

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

        [Required(ErrorMessage = "Username cannot be empty!")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Empty password not allowed!")]
        [DataType(DataType.Password)]
        public byte[] Password { get; set; }
        public string Role { get; set; }

        public virtual ICollection<Result> Result { get; set; }
    }
}
