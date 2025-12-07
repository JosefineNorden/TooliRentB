using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TooLiRent.Services.DTOs.CustomerDTOs;

namespace TooLiRent.Services.Validation
{
    public class CustomerCreateDtoValidator : AbstractValidator<CustomerCreateDto>
    {
        public CustomerCreateDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Namn är obligatoriskt")
                .MaximumLength(50);

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("E-postadress är obligatorisk")
                .EmailAddress().WithMessage("Ogiltig e-postadress");

            RuleFor(x => x.PhoneNumber)
                .NotEmpty().WithMessage("Telefonnummer är obligatoriskt")
                .Matches(@"^\d{10}$").WithMessage("Telefonnummer måste vara 10 siffror");
        }
    }
}
