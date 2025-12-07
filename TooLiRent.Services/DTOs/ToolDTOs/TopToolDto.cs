using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TooLiRent.Services.DTOs.ToolDTOs
{
    public class TopToolDto
    {
        public int ToolId { get; set; }
        public string ToolName { get; set; } = string.Empty;
        public int RentalsCount { get; set; }
        public int UnitsRented { get; set; }
    }
}
