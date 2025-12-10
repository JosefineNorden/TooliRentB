using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TooLiRent.Services.DTOs.RentalDTOs
{
    public class RentalCreateMemberDto
    {
        public List<RentalToolItemDto> Tools { get; set; } = new();
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
