using CodeLifeAcademy.Application.DTOs;
using FluentValidation;
namespace CodeLifeAcademy.Application.Validators;

public class CreateTopicValidator: AbstractValidator<CreateTopicDto>
{
    public CreateTopicValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Название обязательно")
            .MaximumLength(100).WithMessage("Название не должно превышать 100 символов");

        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("Описание не должно превышать 500 символов");

        RuleFor(x => x.CourseId)
            .NotNull().WithMessage("Обязателен к добавлению");
    }
}
