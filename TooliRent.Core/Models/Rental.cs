using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TooliRent.Core.Models
{
    public class Rental : BaseEntity
    {
        public int CustomerId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public Customer Customer { get; set; } = null!;
        public ICollection<RentalDetail> RentalDetails { get; set; } = new List<RentalDetail>();
        public bool IsReturned { get; set; } = false;
    }
}
