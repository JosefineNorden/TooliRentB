using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TooLiRent.Core.Enums;
using TooLiRent.Services.DTOs.ToolDTOs;
using TooLiRent.Services.Interfaces;

namespace TooliRentB.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ToolController : ControllerBase
    {
        private readonly IToolService _service;

        public ToolController(IToolService service)
        {
            _service = service;
        }

        // GET: /api/tools
        // Stöd för filtrering via query
        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<ToolDto>>> GetAll(
            [FromQuery] string? name,
            [FromQuery] int? categoryId,
            [FromQuery] ToolStatus? status,
            [FromQuery] bool? onlyAvailable,
            [FromQuery] DateTime? from,
            [FromQuery] DateTime? to,
            CancellationToken ct)
        {
            // Om inga filter alls -> hämta alla
            var noFilters = name is null && categoryId is null && status is null && onlyAvailable is null && from is null && to is null;
            if (noFilters)
            {
                var all = await _service.GetAllAsync(ct);
                return Ok(all);
            }

            var filtered = await _service.FilterAsync(name, categoryId, status, onlyAvailable, from, to, ct);
            return Ok(filtered);
        }

        // GET: /api/tools/5
        [HttpGet("{id:int}")]
        public async Task<ActionResult<ToolDto>> GetById(int id, CancellationToken ct)
        {
            var item = await _service.GetByIdAsync(id, ct);
            return item is null ? NotFound() : Ok(item);
        }

        // GET: /api/tools/5/available?from=2025-10-01&to=2025-10-03
        [HttpGet("{id:int}/available")]
        public async Task<ActionResult<bool>> IsAvailable(int id, DateTime? from, DateTime? to, CancellationToken ct)
        {
            var exists = await _service.GetByIdAsync(id, ct);
            if (exists is null) return NotFound();
            var ok = await _service.IsAvailableAsync(id, from, to, ct);
            return Ok(ok);
        }

        // GET: /api/tools/status/Available
        [HttpGet("status/{status}")]
        public async Task<ActionResult<IReadOnlyList<ToolDto>>> GetByStatus(ToolStatus status, CancellationToken ct)
        {
            var items = await _service.GetByStatusAsync(status, ct);
            return Ok(items);
        }

        // POST: /api/tools
        [HttpPost]
        public async Task<ActionResult<ToolDto>> Create([FromBody] ToolCreateDto dto, CancellationToken ct)
        {
            try
            {
                var created = await _service.CreateAsync(dto, ct);
                return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
            }
            catch (ValidationException vex)
            {
                return ValidationProblem(ToProblemDetails(vex));
            }
        }

        // PUT: /api/tools/5
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] ToolUpdateDto dto, CancellationToken ct)
        {
            try
            {
                var ok = await _service.UpdateAsync(id, dto, ct);
                return ok ? NoContent() : NotFound();
            }
            catch (ValidationException vex)
            {
                return ValidationProblem(ToProblemDetails(vex));
            }
        }

        // DELETE: /api/tools/5
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id, CancellationToken ct)
        {
            var ok = await _service.DeleteAsync(id, ct);
            return ok ? NoContent() : NotFound();
        }

        // --- helpers ---
        private static ValidationProblemDetails ToProblemDetails(ValidationException vex)
        {
            var errors = vex.Errors
                .GroupBy(e => e.PropertyName)
                .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());

            return new ValidationProblemDetails(errors)
            {
                Title = "Validation failed",
                Status = StatusCodes.Status400BadRequest
            };
        }
    }

}
