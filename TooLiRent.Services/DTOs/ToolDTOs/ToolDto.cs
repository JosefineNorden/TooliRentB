using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TooliRent.Core.Enums;
using TooliRent.Core.Models;

namespace TooLiRent.Services.DTOs.ToolDTOs
{
    public class ToolDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public string? CatalogNumber { get; set; }
        public ToolStatus Status { get; set; }
        public int CategoryId { get; set; }
        public string? CategoryName { get; set; } // mappas via Include/Select
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
