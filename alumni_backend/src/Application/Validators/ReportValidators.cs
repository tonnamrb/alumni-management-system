using Application.DTOs.Reports;
using FluentValidation;

namespace Application.Validators;

public class CreateReportDtoValidator : AbstractValidator<CreateReportDto>
{
    public CreateReportDtoValidator()
    {
        RuleFor(x => x.Type)
            .IsInEnum()
            .WithMessage("Invalid report type");

        RuleFor(x => x.Reason)
            .NotEmpty()
            .WithMessage("Reason is required")
            .MaximumLength(500)
            .WithMessage("Reason cannot exceed 500 characters");

        RuleFor(x => x.AdditionalDetails)
            .MaximumLength(1000)
            .WithMessage("Additional details cannot exceed 1000 characters")
            .When(x => !string.IsNullOrEmpty(x.AdditionalDetails));

        RuleFor(x => x.PostId)
            .GreaterThan(0)
            .WithMessage("PostId must be greater than 0")
            .When(x => x.PostId.HasValue);

        RuleFor(x => x.CommentId)
            .GreaterThan(0)
            .WithMessage("CommentId must be greater than 0")
            .When(x => x.CommentId.HasValue);

        // Custom validation: Either PostId or CommentId must be provided, but not both
        RuleFor(x => x)
            .Must(x => x.PostId.HasValue || x.CommentId.HasValue)
            .WithMessage("Either PostId or CommentId must be provided")
            .WithName("Content");

        RuleFor(x => x)
            .Must(x => !(x.PostId.HasValue && x.CommentId.HasValue))
            .WithMessage("Cannot report both Post and Comment in the same report")
            .WithName("Content");
    }
}

public class ResolveReportDtoValidator : AbstractValidator<ResolveReportDto>
{
    public ResolveReportDtoValidator()
    {
        RuleFor(x => x.Status)
            .IsInEnum()
            .WithMessage("Invalid report status")
            .Must(status => status == Domain.Enums.ReportStatus.Resolved || 
                           status == Domain.Enums.ReportStatus.Dismissed)
            .WithMessage("Report can only be resolved or dismissed");

        RuleFor(x => x.ResolutionNote)
            .NotEmpty()
            .WithMessage("Resolution note is required")
            .MaximumLength(1000)
            .WithMessage("Resolution note cannot exceed 1000 characters");
    }
}