using AutoMapper;
using Domain.Entities;
using Application.DTOs;
using Application.DTOs.Reports;

namespace Application.Mappings;

public class ReportMappingProfile : Profile
{
    public ReportMappingProfile()
    {
        // Report Mappings - ใช้ mapping แบบ manual ใน service เพื่อความยืดหยุ่น
        CreateMap<Report, ReportDto>()
            .ForMember(dest => dest.Reporter, opt => opt.Ignore()) // จะ map ใน service
            .ForMember(dest => dest.ResolvedByUser, opt => opt.Ignore()) // จะ map ใน service
            .ForMember(dest => dest.ReportedContent, opt => opt.Ignore()) // จะ map ใน service
            .ForMember(dest => dest.ReportedUserName, opt => opt.Ignore()); // จะ map ใน service
        
        CreateMap<CreateReportDto, Report>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.ReporterId, opt => opt.Ignore()) // จะ set ใน business logic
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => Domain.Enums.ReportStatus.Pending))
            .ForMember(dest => dest.ResolvedByUserId, opt => opt.Ignore())
            .ForMember(dest => dest.ResolutionNote, opt => opt.Ignore())
            .ForMember(dest => dest.ResolvedAt, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.Reporter, opt => opt.Ignore())
            .ForMember(dest => dest.ResolvedByUser, opt => opt.Ignore())
            .ForMember(dest => dest.Post, opt => opt.Ignore())
            .ForMember(dest => dest.Comment, opt => opt.Ignore());
    }
}