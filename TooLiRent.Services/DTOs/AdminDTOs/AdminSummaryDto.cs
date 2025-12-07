using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TooLiRent.Services.DTOs.AdminDTOs
{
    public class AdminSummaryDto
    {
        public int ToolsTotal { get; set; }
        public int ToolsAvailable { get; set; }
        public int ToolsRented { get; set; }
        public int ToolsBroken { get; set; }

        public int RentalsTotal { get; set; }
        public int ActiveRentals { get; set; }
        public int OverdueCount { get; set; }
    }
}
