using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TooLiRent.Services.DTOs.RentalDTOs
{
    public class RentalCreateDto
    {
        public int CustomerId { get; set; }
        public List<int> ToolIds { get; set; } = new();
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
