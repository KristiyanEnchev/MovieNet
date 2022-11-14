namespace Application.Handlers.Movies.Queries.Validators
{
    using FluentValidation;

    public class GetMovieDetailsQueryValidator : AbstractValidator<GetMovieDetailsQuery>
    {
        public GetMovieDetailsQueryValidator()
        {
            RuleFor(x => x.TmdbId)
                .GreaterThan(0);
        }
    }
}