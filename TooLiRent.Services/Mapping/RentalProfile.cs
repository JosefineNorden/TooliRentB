using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TooLiRent.Core.Models;
using TooLiRent.Services.DTOs.RentalDTOs;

namespace TooLiRent.Services.Mapping
{
    public class RentalProfile : Profile
    {
        public RentalProfile()
        {
            // Entity -> DTO
            CreateMap<Rental, RentalDto>()
                .ForMember(d => d.CustomerName,
                    opt => opt.MapFrom(s => s.Customer != null ? s.Customer.Name : string.Empty))
                .ForMember(d => d.ToolNames,
                    opt => opt.MapFrom(s =>
                        s.RentalDetails != null
                            ? s.RentalDetails.Select(rd => rd.Tool.Name).ToList()
                            : new System.Collections.Generic.List<string>()))

                // IsLate
                .ForMember(d => d.IsLate,
                    opt => opt.MapFrom(s =>
                        !s.IsReturned &&
                        (
                            s.EndDate.Kind == DateTimeKind.Utc
                                ? s.EndDate
                                : s.EndDate.ToUniversalTime()
                        ) < DateTime.UtcNow
                    ))

                // LateMinutes
                .ForMember(d => d.LateMinutes,
                    opt => opt.MapFrom(s =>
                        s.IsReturned
                            ? 0
                            : (int)(
                                DateTime.UtcNow -
                                (
                                    s.EndDate.Kind == DateTimeKind.Utc
                                        ? s.EndDate
                                        : s.EndDate.ToUniversalTime()
                                )
                            ).TotalMinutes
                    ))

                // LateFee 
                .ForMember(d => d.LateFee,
                    opt => opt.Ignore());

            // DTO -> Entity
            CreateMap<RentalCreateDto, Rental>();
            CreateMap<RentalUpdateDto, Rental>();
        }
    }
}
