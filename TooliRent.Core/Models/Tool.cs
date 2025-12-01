using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TooliRent.Core.Enums;

namespace TooliRent.Core.Models
{
    public class Tool : BaseEntity
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        public int Price { get; set; }

        [MaxLength(1000)]
        public string Description { get; set; } = string.Empty;

        public int Stock { get; set; }

        [MaxLength(20)]
        public string? CatalogNumber { get; set; }

        public ToolStatus Status { get; set; } = ToolStatus.Available;

        public int CategoryId { get; set; }

        public Category? Category { get; set; }
    }
}
