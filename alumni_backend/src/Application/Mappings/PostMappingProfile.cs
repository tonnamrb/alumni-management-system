using AutoMapper;
using Domain.Entities;
using Application.DTOs.Posts;

namespace Application.Mappings;

public class PostMappingProfile : Profile
{
    public PostMappingProfile()
    {
        // Post Mappings
        CreateMap<Post, PostDto>()
            .ForMember(dest => dest.LikesCount, opt => opt.MapFrom(src => src.Likes.Count))
            .ForMember(dest => dest.CommentsCount, opt => opt.MapFrom(src => src.Comments.Count))
            .ForMember(dest => dest.IsLikedByCurrentUser, opt => opt.Ignore()); // จะ set ใน business logic
        
        CreateMap<CreatePostDto, Post>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.UserId, opt => opt.Ignore()) // จะ set ใน business logic
            .ForMember(dest => dest.IsPinned, opt => opt.MapFrom(src => false))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.User, opt => opt.Ignore())
            .ForMember(dest => dest.Comments, opt => opt.Ignore())
            .ForMember(dest => dest.Likes, opt => opt.Ignore())
            .ForMember(dest => dest.Reports, opt => opt.Ignore());
        
        CreateMap<UpdatePostDto, Post>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.UserId, opt => opt.Ignore())
            .ForMember(dest => dest.IsPinned, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.User, opt => opt.Ignore())
            .ForMember(dest => dest.Comments, opt => opt.Ignore())
            .ForMember(dest => dest.Likes, opt => opt.Ignore())
            .ForMember(dest => dest.Reports, opt => opt.Ignore());
    }
}