using AutoMapper;
using Domain.Entities;
using Application.DTOs;

namespace Application.Mappings;

public class UserMappingProfile : Profile
{
    public UserMappingProfile()
    {
        // User Entity → UserDto (Updated for new schema)
        CreateMap<User, UserDto>()
            .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.FullName))
            .ForMember(dest => dest.RoleName, opt => opt.MapFrom(src => src.Role != null ? src.Role.Name : "Unknown"))
            .ForMember(dest => dest.IsMember, opt => opt.MapFrom(src => src.RoleId == 1))
            .ForMember(dest => dest.IsAdmin, opt => opt.MapFrom(src => src.RoleId == 2));

        // CreateUserDto → User (Simplified for compatibility)
        CreateMap<CreateUserDto, User>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Firstname, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.Lastname, opt => opt.MapFrom(src => ""))
            .ForMember(dest => dest.PasswordHash, opt => opt.Ignore()) // Will be hashed in business logic
            .ForMember(dest => dest.RoleId, opt => opt.MapFrom(src => 1)) // Default to Member role
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.Role, opt => opt.Ignore())
            .ForMember(dest => dest.AlumniProfile, opt => opt.Ignore())
            .ForMember(dest => dest.Posts, opt => opt.Ignore())
            .ForMember(dest => dest.Comments, opt => opt.Ignore())
            .ForMember(dest => dest.Likes, opt => opt.Ignore())
            .ForMember(dest => dest.Reports, opt => opt.Ignore())
            .ForMember(dest => dest.ResolvedReports, opt => opt.Ignore());
        
        // UpdateUserDto → User (Simplified for compatibility)
        CreateMap<UpdateUserDto, User>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Firstname, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.Lastname, opt => opt.MapFrom(src => ""))
            .ForMember(dest => dest.PasswordHash, opt => opt.Ignore())
            .ForMember(dest => dest.Role, opt => opt.Ignore())
            .ForMember(dest => dest.RoleId, opt => opt.Ignore())
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