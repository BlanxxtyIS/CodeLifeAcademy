using CodeLifeAcademy.Application.DTOs;
using FluentValidation;

namespace CodeLifeAcademy.Application.Validators;

public class CreateLessionValidator: AbstractValidator<CreateLessionDto>
{
    public CreateLessionValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Название обязательно")
            .MaximumLength(100).WithMessage("Название не должно превышать 100 символов");

        RuleFor(x => x.Content)
            .MaximumLength(5000).WithMessage("Описание не должно превышать 5000 символов");

        RuleFor(x => x.TopicId)
            .NotNull().WithMessage("Обязателен к добавлению");
    }
}
