using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TooLiRent.Services.DTOs.RentalDTOs;

namespace TooLiRent.Services.Validation
{
    public class RentalUpdateDtoValidator : AbstractValidator<RentalUpdateDto>
    {

        public RentalUpdateDtoValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("ID krävs.");

            RuleFor(x => x.StartDate)
                .NotEmpty().WithMessage("Startdatum krävs.");

            RuleFor(x => x.EndDate)
                .NotEmpty().WithMessage("Slutdatum krävs.")
                .GreaterThan(x => x.StartDate).WithMessage("Slutdatum måste vara efter startdatum.");
        }
    }
}
