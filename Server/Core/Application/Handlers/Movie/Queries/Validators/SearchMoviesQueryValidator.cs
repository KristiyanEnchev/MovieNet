namespace Application.Handlers.Movies.Queries.Validators
{
    using FluentValidation;

    public class SearchMoviesQueryValidator : AbstractValidator<SearchMoviesQuery>
    {
        public SearchMoviesQueryValidator()
        {
            RuleFor(x => x.Query)
                .NotEmpty()
                .MinimumLength(2)
                .MaximumLength(100);

            RuleFor(x => x.Page)
                .GreaterThan(0);
        }
    }
}
