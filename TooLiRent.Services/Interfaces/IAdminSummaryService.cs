using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TooLiRent.Services.DTOs.AdminDTOs;
using TooLiRent.Services.DTOs.ToolDTOs;

namespace TooLiRent.Services.Interfaces
{
    public interface IAdminSummaryService
    {
        Task<AdminSummaryDto> GetSummaryAsync();

        Task<IEnumerable<TopToolDto>> GetTopToolsAsync(
            DateTime? from = null,
            DateTime? to = null,
            int take = 3);
    }
}
