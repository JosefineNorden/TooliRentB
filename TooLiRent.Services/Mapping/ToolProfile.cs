using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TooliRent.Core.Models;
using TooLiRent.Services.DTOs;

namespace TooLiRent.Services.Mapping
{
    public class ToolProfile : Profile
    {
        public ToolProfile()
        {
            CreateMap<Tool, ToolDto>()
                .ForMember(d => d.CategoryId, o => o.MapFrom(s => s.CategoryId))
                .ForMember(d => d.CategoryName, o => o.MapFrom(s => s.Category.Name));

            CreateMap<ToolCreateDto, Tool>()
                .ForMember(d => d.Id, o => o.Ignore());

            CreateMap<ToolUpdateDto, Tool>()
                .ForMember(d => d.Id, o => o.Ignore());
        }
    }
}
