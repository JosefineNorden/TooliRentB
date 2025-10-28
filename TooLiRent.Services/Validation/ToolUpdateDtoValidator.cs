using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TooLiRent.Services.DTOs;

namespace TooLiRent.Services.Validation
{
    public class ToolUpdateDtoValidator : AbstractValidator<ToolUpdateDto>
    {
        public ToolUpdateDtoValidator() => Include(new ToolCreateDtoValidator());
    }
}
