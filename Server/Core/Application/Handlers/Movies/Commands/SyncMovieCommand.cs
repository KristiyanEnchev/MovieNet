namespace Application.Handlers.Movies.Commands
{
    using MediatR;

    using Application.Interfaces;

    using Models.Movie;

    using Domain.Enums;

    using Shared;

    public record SyncMovieCommand : IRequest<Result<MovieDetailsDto>>
    {
        public MediaType MediaType { get; init; }
        public int TmdbId { get; init; }
        public bool AppendToResponse { get; init; }

        public SyncMovieCommand(MediaType mediaType, int tmdbId, bool appendToResponse = false)
        {
            MediaType = mediaType;
            TmdbId = tmdbId;
            AppendToResponse = appendToResponse;
        }

        public class SyncMovieCommandHandler : IRequestHandler<SyncMovieCommand, Result<MovieDetailsDto>>
        {
            private readonly IMovieService _movieService;

            public SyncMovieCommandHandler(IMovieService movieService)
            {
                _movieService = movieService;
            }

            public async Task<Result<MovieDetailsDto>> Handle(SyncMovieCommand request, CancellationToken cancellationToken)
            {
                return await _movieService.SyncMovieFromTmdbAsync(request.MediaType, request.TmdbId, request.AppendToResponse, cancellationToken);
            }
        }
    }
}