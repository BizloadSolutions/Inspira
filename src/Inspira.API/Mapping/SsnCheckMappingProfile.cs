using AutoMapper;
using Inspira.Application.Services;
using Inspira.API.Models;

namespace Inspira.API.Mapping
{
    public class SsnCheckMappingProfile : Profile
    {
        public SsnCheckMappingProfile()
        {
            CreateMap<SsnCheckServiceResult, SsnCheckResponseDto>();
        }
    }
}
