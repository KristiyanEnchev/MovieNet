namespace Application.Handlers.Movies.Queries
{
    using Application.Interfaces;

    using Domain.Enums;

    using MediatR;

    using Models.Tmdb;

    using Shared;

    public record GetMovieVideosQuery : IRequest<Result<TmdbVideoResultsDto>>
    {
        public MediaType MediaType { get; init; }
        public int TmdbId { get; init; }

        public GetMovieVideosQuery(MediaType mediaType, int tmdbId)
        {
            MediaType = mediaType;
            TmdbId = tmdbId;
        }

        public class GetMovieVideosQueryHandler : IRequestHandler<GetMovieVideosQuery, Result<TmdbVideoResultsDto>>
        {
            private readonly ITmdbService _tmdbService;

            public GetMovieVideosQueryHandler(ITmdbService tmdbService)
            {
                _tmdbService = tmdbService;
            }

            public async Task<Result<TmdbVideoResultsDto>> Handle(GetMovieVideosQuery request, CancellationToken cancellationToken)
            {
                return await _tmdbService.GetVideosAsync(request.MediaType, request.TmdbId, cancellationToken);
            }
        }
    }
}
