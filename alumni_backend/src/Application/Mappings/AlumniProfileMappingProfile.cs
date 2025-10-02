using AutoMapper;
using Domain.Entities;
using Application.DTOs;

namespace Application.Mappings;

public class AlumniProfileMappingProfile : Profile
{
    public AlumniProfileMappingProfile()
    {
        // AlumniProfile Mappings
        CreateMap<AlumniProfile, AlumniProfileDto>();
        
        CreateMap<CreateAlumniProfileDto, AlumniProfile>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.UserId, opt => opt.Ignore()) // จะ set ใน business logic
            .ForMember(dest => dest.ProfilePictureUrl, opt => opt.Ignore()) // จะอัพโหลดแยก
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.User, opt => opt.Ignore());
        
        CreateMap<UpdateAlumniProfileDto, AlumniProfile>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.UserId, opt => opt.Ignore())
            .ForMember(dest => dest.ProfilePictureUrl, opt => opt.Ignore()) // จะอัพโหลดแยก
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.User, opt => opt.Ignore());
    }
}