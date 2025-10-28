using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TooliRent.Core.Enums;

namespace TooLiRent.Services.DTOs
{
    public class ToolAvailabilityDto
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public string? CatalogNumber { get; set; }
        public ToolStatus Status { get; set; }
        public int CategoryId { get; set; }
    }
}
