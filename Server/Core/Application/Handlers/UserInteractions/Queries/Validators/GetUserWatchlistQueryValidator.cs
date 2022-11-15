namespace Application.Handlers.UserInteractions.Queries.Validators
{
    using FluentValidation;

    public class GetUserWatchlistQueryValidator : AbstractValidator<GetUserWatchlistQuery>
    {
        public GetUserWatchlistQueryValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty();

            RuleFor(x => x.PageNumber)
                .GreaterThan(0);

            RuleFor(x => x.PageSize)
                .InclusiveBetween(1, 100);
        }
    }
}
