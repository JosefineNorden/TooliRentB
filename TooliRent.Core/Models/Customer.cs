using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TooLiRent.Core.Enums;

namespace TooLiRent.Core.Models
{
    public class Customer : BaseEntity
    {
        [Required]
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public CustomerStatus Status { get; set; } = CustomerStatus.Active;
    }
}
