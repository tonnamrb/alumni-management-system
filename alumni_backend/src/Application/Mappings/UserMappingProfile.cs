using AutoMapper;
using Domain.Entities;
using Application.DTOs;

namespace Application.Mappings;

public class UserMappingProfile : Profile
{
    public UserMappingProfile()
    {
        // User Mappings
        CreateMap<User, UserDto>()
            .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Role.ToString()));
        
        CreateMap<CreateUserDto, User>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.PasswordHash, opt => opt.MapFrom(src => src.Password)) // จะ hash ใน business logic
            .ForMember(dest => dest.Role, opt => opt.MapFrom(src => Domain.Enums.UserRole.Alumni))
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.AlumniProfile, opt => opt.Ignore())
            .ForMember(dest => dest.Posts, opt => opt.Ignore())
            .ForMember(dest => dest.Comments, opt => opt.Ignore())
            .ForMember(dest => dest.Likes, opt => opt.Ignore())
            .ForMember(dest => dest.Reports, opt => opt.Ignore())
            .ForMember(dest => dest.ResolvedReports, opt => opt.Ignore());
        
        CreateMap<UpdateUserDto, User>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.PasswordHash, opt => opt.Ignore())
            .ForMember(dest => dest.Role, opt => opt.Ignore())
            .ForMember(dest => dest.Provider, opt => opt.Ignore())
            .ForMember(dest => dest.ProviderId, opt => opt.Ignore())
            .ForMember(dest => dest.LastLoginAt, opt => opt.Ignore())
            .ForMember(dest => dest.IsActive, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.AlumniProfile, opt => opt.Ignore())
            .ForMember(dest => dest.Posts, opt => opt.Ignore())
            .ForMember(dest => dest.Comments, opt => opt.Ignore())
            .ForMember(dest => dest.Likes, opt => opt.Ignore())
            .ForMember(dest => dest.Reports, opt => opt.Ignore())
            .ForMember(dest => dest.ResolvedReports, opt => opt.Ignore());
    }
}