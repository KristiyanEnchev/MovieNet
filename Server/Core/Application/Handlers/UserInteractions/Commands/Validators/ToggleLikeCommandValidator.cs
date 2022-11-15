namespace Application.Handlers.UserInteractions.Commands.Validators
{
    using FluentValidation;

    public class ToggleLikeCommandValidator : AbstractValidator<ToggleLikeCommand>
    {
        public ToggleLikeCommandValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty();

            RuleFor(x => x.MovieId)
                .NotEmpty();
        }
    }
}
