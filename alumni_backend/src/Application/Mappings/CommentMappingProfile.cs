using AutoMapper;
using Domain.Entities;
using Application.DTOs.Comments;
using Application.DTOs.Likes;

namespace Application.Mappings;

public class CommentMappingProfile : Profile
{
    public CommentMappingProfile()
    {
        // Comment Mappings - Note: Custom mapping used in service layer
        CreateMap<Comment, CommentDto>()
            .ForMember(dest => dest.LikesCount, opt => opt.Ignore())
            .ForMember(dest => dest.IsLikedByCurrentUser, opt => opt.Ignore())
            .ForMember(dest => dest.Replies, opt => opt.Ignore())
            .ForMember(dest => dest.MentionedUsers, opt => opt.Ignore());
        
        CreateMap<CreateCommentDto, Comment>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.UserId, opt => opt.Ignore()) // จะ set ใน business logic
            .ForMember(dest => dest.MentionedUserIds, opt => opt.MapFrom(src => 
                src.MentionedUserIds != null ? string.Join(",", src.MentionedUserIds) : null))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.User, opt => opt.Ignore())
            .ForMember(dest => dest.Post, opt => opt.Ignore())
            .ForMember(dest => dest.ParentComment, opt => opt.Ignore())
            .ForMember(dest => dest.Replies, opt => opt.Ignore())
            .ForMember(dest => dest.Reports, opt => opt.Ignore());
        
        CreateMap<UpdateCommentDto, Comment>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.UserId, opt => opt.Ignore())
            .ForMember(dest => dest.PostId, opt => opt.Ignore())
            .ForMember(dest => dest.ParentCommentId, opt => opt.Ignore())
            .ForMember(dest => dest.MentionedUserIds, opt => opt.MapFrom(src => 
                src.MentionedUserIds != null ? string.Join(",", src.MentionedUserIds) : null))
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.User, opt => opt.Ignore())
            .ForMember(dest => dest.Post, opt => opt.Ignore())
            .ForMember(dest => dest.ParentComment, opt => opt.Ignore())
            .ForMember(dest => dest.Replies, opt => opt.Ignore())
            .ForMember(dest => dest.Reports, opt => opt.Ignore());
            
        // Like Mappings
        CreateMap<Like, LikeDto>();
    }
}