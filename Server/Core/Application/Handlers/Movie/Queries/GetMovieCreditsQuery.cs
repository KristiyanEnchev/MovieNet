namespace Application.Handlers.Movies.Queries
{
    using Application.Interfaces;

    using Domain.Enums;

    using MediatR;

    using Models.Tmdb;

    using Shared;

    public record GetMovieCreditsQuery : IRequest<Result<TmdbCreditsDto>>
    {
        public MediaType MediaType { get; init; }
        public int TmdbId { get; init; }

        public GetMovieCreditsQuery(MediaType mediaType, int tmdbId)
        {
            MediaType = mediaType;
            TmdbId = tmdbId;
        }

        public class GetMovieCreditsQueryHandler : IRequestHandler<GetMovieCreditsQuery, Result<TmdbCreditsDto>>
        {
            private readonly ITmdbService _tmdbService;

            public GetMovieCreditsQueryHandler(ITmdbService tmdbService)
            {
                _tmdbService = tmdbService;
            }

            public async Task<Result<TmdbCreditsDto>> Handle(GetMovieCreditsQuery request, CancellationToken cancellationToken)
            {
                return await _tmdbService.GetCreditsAsync(request.MediaType, request.TmdbId, cancellationToken);
            }
        }
    }
}
