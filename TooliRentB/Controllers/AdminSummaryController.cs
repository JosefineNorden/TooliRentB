using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TooLiRent.Services.DTOs.AdminDTOs;
using TooLiRent.Services.DTOs.ToolDTOs;
using TooLiRent.Services.Interfaces;

namespace TooLiRentB.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class AdminSummaryController : ControllerBase
    {
        private readonly IAdminSummaryService _stats;

        public AdminSummaryController(IAdminSummaryService stats)
        {
            _stats = stats;
        }

        [HttpGet("summary")]
        [ProducesResponseType(typeof(AdminSummaryDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> Summary()
        {
            var dto = await _stats.GetSummaryAsync();
            return Ok(dto);
        }

        [HttpGet("top-tools")]
        [ProducesResponseType(typeof(IEnumerable<TopToolDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> TopTools([FromQuery] DateTime? from, [FromQuery] DateTime? to, [FromQuery] int take = 5)
        {
            if (take <= 0) take = 5;
            var list = await _stats.GetTopToolsAsync(from, to, take);
            return Ok(list);
        }
    }
}
