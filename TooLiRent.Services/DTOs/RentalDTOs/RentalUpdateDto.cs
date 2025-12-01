using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TooLiRent.Services.DTOs.RentalDTOs
{
    public class RentalUpdateDto
    {
        public int Id { get; set; } // krävs för att hitta rätt uthyrning

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }
    }
}
