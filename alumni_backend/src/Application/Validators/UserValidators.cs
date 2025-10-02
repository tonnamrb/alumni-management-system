using Application.DTOs;
using FluentValidation;

namespace Application.Validators;

public class CreateUserDtoValidator : AbstractValidator<CreateUserDto>
{
    public CreateUserDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("ชื่อเป็นข้อมูลที่จำเป็น")
            .MaximumLength(255).WithMessage("ชื่อต้องไม่เกิน 255 ตัวอักษร");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("อีเมลเป็นข้อมูลที่จำเป็น")
            .EmailAddress().WithMessage("รูปแบบอีเมลไม่ถูกต้อง")
            .MaximumLength(255).WithMessage("อีเมลต้องไม่เกิน 255 ตัวอักษร");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("รหัสผ่านเป็นข้อมูลที่จำเป็น")
            .MinimumLength(6).WithMessage("รหัสผ่านต้องมีความยาวอย่างน้อย 6 ตัวอักษร")
            .MaximumLength(100).WithMessage("รหัสผ่านต้องไม่เกิน 100 ตัวอักษร")
            .When(x => string.IsNullOrEmpty(x.Provider)); // ไม่ต้องการรหัสผ่านสำหรับ OAuth

        RuleFor(x => x.Provider)
            .MaximumLength(50).WithMessage("Provider ต้องไม่เกิน 50 ตัวอักษร")
            .When(x => !string.IsNullOrEmpty(x.Provider));

        RuleFor(x => x.ProviderId)
            .MaximumLength(255).WithMessage("Provider ID ต้องไม่เกิน 255 ตัวอักษร")
            .When(x => !string.IsNullOrEmpty(x.ProviderId));
    }
}

public class UpdateUserDtoValidator : AbstractValidator<UpdateUserDto>
{
    public UpdateUserDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("ชื่อเป็นข้อมูลที่จำเป็น")
            .MaximumLength(255).WithMessage("ชื่อต้องไม่เกิน 255 ตัวอักษร");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("อีเมลเป็นข้อมูลที่จำเป็น")
            .EmailAddress().WithMessage("รูปแบบอีเมลไม่ถูกต้อง")
            .MaximumLength(255).WithMessage("อีเมลต้องไม่เกิน 255 ตัวอักษร");
    }
}

public class LoginDtoValidator : AbstractValidator<LoginDto>
{
    public LoginDtoValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("อีเมลเป็นข้อมูลที่จำเป็น")
            .EmailAddress().WithMessage("รูปแบบอีเมลไม่ถูกต้อง");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("รหัสผ่านเป็นข้อมูลที่จำเป็น");
    }
}

public class ChangePasswordDtoValidator : AbstractValidator<ChangePasswordDto>
{
    public ChangePasswordDtoValidator()
    {
        RuleFor(x => x.CurrentPassword)
            .NotEmpty().WithMessage("รหัสผ่านปัจจุบันเป็นข้อมูลที่จำเป็น");

        RuleFor(x => x.NewPassword)
            .NotEmpty().WithMessage("รหัสผ่านใหม่เป็นข้อมูลที่จำเป็น")
            .MinimumLength(6).WithMessage("รหัสผ่านใหม่ต้องมีความยาวอย่างน้อย 6 ตัวอักษร")
            .MaximumLength(100).WithMessage("รหัสผ่านใหม่ต้องไม่เกิน 100 ตัวอักษร");

        RuleFor(x => x.ConfirmPassword)
            .NotEmpty().WithMessage("การยืนยันรหัสผ่านเป็นข้อมูลที่จำเป็น")
            .Equal(x => x.NewPassword).WithMessage("การยืนยันรหัสผ่านไม่ตรงกับรหัสผ่านใหม่");
    }
}