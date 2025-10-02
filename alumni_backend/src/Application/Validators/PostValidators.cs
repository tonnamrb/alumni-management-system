using FluentValidation;
using Application.DTOs.Posts;
using Application.DTOs.Comments;

namespace Application.Validators;

public class CreatePostDtoValidator : AbstractValidator<CreatePostDto>
{
    public CreatePostDtoValidator()
    {
        RuleFor(x => x.Content)
            .NotEmpty().WithMessage("เนื้อหาโพสต์จำเป็นต้องระบุ")
            .MaximumLength(2000).WithMessage("เนื้อหาโพสต์ต้องไม่เกิน 2000 ตัวอักษร");

        RuleFor(x => x.ImageUrl)
            .Must(BeAValidUrl).WithMessage("URL รูปภาพไม่ถูกต้อง")
            .When(x => !string.IsNullOrEmpty(x.ImageUrl));
    }

    private bool BeAValidUrl(string url)
    {
        return Uri.TryCreate(url, UriKind.Absolute, out _);
    }
}

public class UpdatePostDtoValidator : AbstractValidator<UpdatePostDto>
{
    public UpdatePostDtoValidator()
    {
        RuleFor(x => x.Content)
            .NotEmpty().WithMessage("เนื้อหาโพสต์จำเป็นต้องระบุ")
            .MaximumLength(2000).WithMessage("เนื้อหาโพสต์ต้องไม่เกิน 2000 ตัวอักษร");

        RuleFor(x => x.ImageUrl)
            .Must(BeAValidUrl).WithMessage("URL รูปภาพไม่ถูกต้อง")
            .When(x => !string.IsNullOrEmpty(x.ImageUrl));
    }

    private bool BeAValidUrl(string url)
    {
        return Uri.TryCreate(url, UriKind.Absolute, out _);
    }
}

public class CreateCommentDtoValidator : AbstractValidator<CreateCommentDto>
{
    public CreateCommentDtoValidator()
    {
        RuleFor(x => x.PostId)
            .GreaterThan(0).WithMessage("Post ID จำเป็นต้องระบุ");

        RuleFor(x => x.Content)
            .NotEmpty().WithMessage("เนื้อหาคอมเมนต์จำเป็นต้องระบุ")
            .MaximumLength(1000).WithMessage("เนื้อหาคอมเมนต์ต้องไม่เกิน 1000 ตัวอักษร");

        RuleFor(x => x.ParentCommentId)
            .GreaterThan(0).WithMessage("Parent Comment ID ต้องมากกว่า 0")
            .When(x => x.ParentCommentId.HasValue);

        RuleFor(x => x.MentionedUserIds)
            .Must(BeValidUserIds).WithMessage("รายการ User ID ที่ mention ไม่ถูกต้อง")
            .When(x => x.MentionedUserIds != null);
    }

    private bool BeValidUserIds(List<int>? userIds)
    {
        return userIds == null || userIds.All(id => id > 0);
    }
}

public class UpdateCommentDtoValidator : AbstractValidator<UpdateCommentDto>
{
    public UpdateCommentDtoValidator()
    {
        RuleFor(x => x.Content)
            .NotEmpty().WithMessage("เนื้อหาคอมเมนต์จำเป็นต้องระบุ")
            .MaximumLength(1000).WithMessage("เนื้อหาคอมเมนต์ต้องไม่เกิน 1000 ตัวอักษร");

        RuleFor(x => x.MentionedUserIds)
            .Must(BeValidUserIds).WithMessage("รายการ User ID ที่ mention ไม่ถูกต้อง")
            .When(x => x.MentionedUserIds != null);
    }

    private bool BeValidUserIds(List<int>? userIds)
    {
        return userIds == null || userIds.All(id => id > 0);
    }
}