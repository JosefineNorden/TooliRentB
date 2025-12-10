using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TooLiRent.Services.DTOs.RentalDTOs;

namespace TooLiRent.Services.Validation
{
    public class RentalCreateDtoValidator : AbstractValidator<RentalCreateDto>
    {
        public RentalCreateDtoValidator()
        {
            RuleFor(x => x.CustomerId)
                .GreaterThan(0).WithMessage("Kund-ID måste vara större än 0.");

            RuleFor(x => x.Tools)
               .NotEmpty().WithMessage("Minst ett verktyg måste väljas.");

            RuleForEach(x => x.Tools).ChildRules(tool =>
            {
                tool.RuleFor(t => t.ToolId).GreaterThan(0);
                tool.RuleFor(t => t.Quantity).GreaterThan(0);
            });

            RuleFor(x => x.StartDate)
                .NotEmpty().WithMessage("Startdatum krävs.");

            RuleFor(x => x.EndDate)
                .NotEmpty().WithMessage("Slutdatum krävs.")
                .GreaterThan(x => x.StartDate).WithMessage("Slutdatum måste vara efter startdatum.");
        }

    }
}
