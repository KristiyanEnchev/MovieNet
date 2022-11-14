namespace Application.Handlers.Movies.Queries.Validators
{
    using FluentValidation;

    using Models.Tmdb.Enums;

    public class GetTrendingMoviesQueryValidator : AbstractValidator<GetTrendingMoviesQuery>
    {
        public GetTrendingMoviesQueryValidator()
        {
            RuleFor(x => x.TimeWindow)
                .Must(x => x == TimeWindow.day || x == TimeWindow.week)
                .WithMessage("TimeWindow must be either 'day' or 'week'");
        }
    }
}