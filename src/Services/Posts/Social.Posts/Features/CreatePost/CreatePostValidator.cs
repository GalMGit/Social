using FluentValidation;

namespace Social.Posts.Features.CreatePost;

public sealed class CreatePostValidator 
    : AbstractValidator<CreatePostRequest>
{
    public CreatePostValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Заголовок не может быть пустым")
            .MaximumLength(200).WithMessage("Заголовок не может быть больше 200 символов");

        RuleFor(x => x.Content)
            .NotEmpty().WithMessage("Содержимое не может быть пустым")
            .MaximumLength(10000).WithMessage("Содержимое не может быть больше 10000 символов");
        
        RuleFor(x => x.ImagePath)
            .MaximumLength(2048)
            .When(x => x.ImagePath is not null);
    }
}