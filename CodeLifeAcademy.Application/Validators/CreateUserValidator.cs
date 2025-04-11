using CodeLifeAcademy.Application.DTOs;
using FluentValidation;

namespace CodeLifeAcademy.Application.Validators;

public class CreateUserValidator : AbstractValidator<CreateUserDto>
{
    public CreateUserValidator()
    {
        RuleFor(x => x.Username)
            .NotEmpty().WithMessage("Имя обязательн")
            .MaximumLength(20).WithMessage("Имя не должно превышать 20 символов");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Почта обязательна")
            .MaximumLength(50).WithMessage("Почта не может содержать более 50 символов")
            .EmailAddress();

        RuleFor(x => x.PasswordHash)
            .NotEmpty().WithMessage("Пароль обязателен")
            .MaximumLength(100).WithMessage("Пароль не может содержать более 100 символов");
    }
}