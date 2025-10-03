using Application.DTOs.Posts;
using Domain.Enums;
using FluentValidation;

namespace Application.Validators.Posts;

public class UpdatePostValidator : AbstractValidator<UpdatePostDto>
{
    public UpdatePostValidator()
    {
        RuleFor(x => x.Content)
            .NotEmpty().WithMessage("Content is required")
            .MaximumLength(2000).WithMessage("Content must not exceed 2000 characters");

        RuleFor(x => x.Type)
            .IsInEnum().WithMessage("Invalid post type");

        RuleFor(x => x.ImageUrl)
            .Must(BeValidUrl).WithMessage("Invalid image URL format")
            .When(x => !string.IsNullOrEmpty(x.ImageUrl));

        RuleFor(x => x.MediaUrls)
            .Must(HaveValidUrls).WithMessage("All media URLs must be valid")
            .When(x => x.MediaUrls != null && x.MediaUrls.Any());

        // Business rules for media based on post type
        RuleFor(x => x)
            .Must(HaveAppropriateMediaForType)
            .WithMessage("Media configuration does not match post type");
    }

    private bool BeValidUrl(string? url)
    {
        if (string.IsNullOrEmpty(url)) return true;
        return Uri.TryCreate(url, UriKind.Absolute, out var result) &&
               (result.Scheme == Uri.UriSchemeHttp || result.Scheme == Uri.UriSchemeHttps);
    }

    private bool HaveValidUrls(List<string>? urls)
    {
        if (urls == null) return true;
        return urls.All(url => BeValidUrl(url));
    }

    private bool HaveAppropriateMediaForType(UpdatePostDto dto)
    {
        return dto.Type switch
        {
            PostType.Text => true, // Text posts can have no media or some media
            PostType.Image => !string.IsNullOrEmpty(dto.ImageUrl) || (dto.MediaUrls?.Count == 1),
            PostType.Album => dto.MediaUrls?.Count > 1,
            PostType.Video => dto.MediaUrls?.Count >= 1, // Video can have multiple media
            _ => false
        };
    }
}