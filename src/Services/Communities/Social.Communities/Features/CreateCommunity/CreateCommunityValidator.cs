using FluentValidation;

namespace Social.Communities.Features.CreateCommunity;

public sealed class CreateCommunityValidator 
    : AbstractValidator<CreateCommunityRequest>
{
    public CreateCommunityValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Название не может быть пустым")
            .MaximumLength(20).WithMessage("Название не может быть больше 20 символов");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Описание не может быть пустым")
            .MaximumLength(200).WithMessage("Описание не может быть больше 200 символов");
    }
}