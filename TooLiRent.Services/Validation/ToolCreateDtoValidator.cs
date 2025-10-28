using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TooLiRent.Services.DTOs;

namespace TooLiRent.Services.Validation
{
    public class ToolCreateDtoValidator : AbstractValidator<ToolCreateDto>
    {
        public ToolCreateDtoValidator()
        {
            RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
            RuleFor(x => x.Price).GreaterThanOrEqualTo(0);
            RuleFor(x => x.Stock).GreaterThanOrEqualTo(0);
            RuleFor(x => x.CatalogNumber).MaximumLength(20).When(x => x.CatalogNumber != null);
            RuleFor(x => x.CategoryId).GreaterThan(0);
            RuleFor(x => x.Status).IsInEnum();
        }
    }
}
