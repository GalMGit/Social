using FluentValidation;

namespace Social.Comments.Features.CreateComment;

public sealed class CreateCommentValidator 
    : AbstractValidator<CreateCommentRequest>
{
    public CreateCommentValidator()
    {
        RuleFor(x => x.Text)
            .NotEmpty().WithMessage("Текст не может быть пустым")
            .MaximumLength(2000).WithMessage("Текст не может быть больше 2000 символов");
    }
}