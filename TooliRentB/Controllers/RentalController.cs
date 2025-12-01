using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TooLiRent.Services.DTOs.RentalDTOs;
using TooLiRent.Services.Interfaces;

namespace TooliRentB.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RentalController : ControllerBase
    {
        private readonly IRentalService _rentalService;

        public RentalController(IRentalService rentalService)
        {
            _rentalService = rentalService;
        }


        private (string? email, bool isAdmin) Caller()
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value
                        ?? User.FindFirst("email")?.Value;
            return (email, User.IsInRole("Admin"));
        }

        /// <summary>
        /// Get all rentals (Admin only)
        /// </summary>
        [Authorize(Roles = "Admin")]
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<RentalDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<RentalDto>>> GetAll()
        {
            var rentals = await _rentalService.GetAllAsync();
            return Ok(rentals);
        }

        /// <summary>
        /// Get rental by ID (ägarkoll i service)
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(RentalDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<RentalDto>> GetById(int id)
        {
            var (email, isAdmin) = Caller();
            try
            {
                var rental = await _rentalService.GetByIdAsync(id, email, isAdmin);
                if (rental == null)
                    return NotFound($"Rental with ID {id} not found");
                return Ok(rental);
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
        }

        /// <summary>
        /// Create a new rental (Member skapar åt sig själv, Admin kan skapa åt valfri kund)
        /// </summary>
        [Authorize(Roles = "Member,Admin")]
        [HttpPost]
        [ProducesResponseType(typeof(RentalDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<RentalDto>> Create([FromBody] RentalCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var (email, isAdmin) = Caller();

            try
            {
                var created = await _rentalService.CreateAsync(dto, email, isAdmin);
                return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
            }
            catch (FluentValidation.ValidationException ex)
{
    var errors = ex.Errors.Select(e => e.ErrorMessage).ToList();
    return BadRequest(new { errors });
}
            catch (InvalidOperationException ex)
            {

                return Conflict(new { error = ex.Message });
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
        }

        /// <summary>
        /// Update an existing rental 
        /// </summary>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(int id, RentalUpdateDto dto)
        {
            if (id != dto.Id)
                return BadRequest("ID i URL matchar inte ID i objektet.");

            var (email, isAdmin) = Caller();
            try
            {
                var updated = await _rentalService.UpdateAsync(dto, email, isAdmin);
                if (updated == null)
                    return NotFound($"Rental with ID {id} not found");

                return Ok(updated);
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
        }


        [Authorize(Roles = "Admin")]
        [HttpPatch("{id:int}/pickup")]
        public async Task<IActionResult> Pickup(int id)
        {
            var email = User.FindFirstValue(ClaimTypes.Email) ?? User.FindFirst("email")?.Value;
            var isAdmin = User.IsInRole("Admin");

            try
            {
                var dto = await _rentalService.PickUpAsync(id, email, isAdmin);
                if (dto is null) return NotFound();
                return Ok(dto);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { error = ex.Message });
            }
        }

        [Authorize(Roles = "Member,Admin")]
        [HttpPatch("{id:int}/return")]
        public async Task<IActionResult> Return(int id)
        {
            var email = User.FindFirstValue(ClaimTypes.Email) ?? User.FindFirst("email")?.Value;
            var isAdmin = User.IsInRole("Admin");

            try
            {
                var dto = await _rentalService.ReturnAsync(id, email, isAdmin);
                if (dto is null) return NotFound();
                return Ok(dto);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { error = ex.Message });
            }
        }

        [Authorize]
        [HttpGet("my")]
        [ProducesResponseType(typeof(IEnumerable<RentalDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<RentalDto>>> MyRentals()
        {
            var (email, _) = Caller();
            if (string.IsNullOrWhiteSpace(email)) return Forbid();
            var items = await _rentalService.GetMyAsync(email);
            return Ok(items);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("overdue")]
        [ProducesResponseType(typeof(IEnumerable<RentalDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<RentalDto>>> Overdue()
        {
            var items = await _rentalService.GetOverdueAsync();
            return Ok(items);
        }


        [Authorize(Roles = "Member,Admin")]
        [HttpDelete("{id:int}/cancel")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Cancel(int id)
        {
            var (email, isAdmin) = Caller();
            try
            {
                var ok = await _rentalService.CancelAsync(id, email, isAdmin);
                if (!ok) return NotFound(); // Bokning hittades inte
                return NoContent();         // Avbokad
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();            // Inte ägare
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message }); // T.ex. "kan inte avbokas efter start"
            }
        }


        /// <summary>
        /// Delete a rental (Admin only)
        /// </summary>
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _rentalService.DeleteAsync(id);
            if (!success)
                return NotFound($"Rental with ID {id} not found");

            return NoContent();
        }

    }
}
