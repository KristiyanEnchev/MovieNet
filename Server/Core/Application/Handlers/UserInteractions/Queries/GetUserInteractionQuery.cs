namespace Application.Handlers.UserInteractions.Queries
{
    using MediatR;

    using Application.Interfaces;

    using Models.Movie;

    using Shared;

    using Domain.Enums;

    public record GetUserInteractionQuery : IRequest<Result<UserMovieInteractionDto>>
    {
        public MediaType MediaType { get; init; }
        public string UserId { get; init; }
        public int MovieId { get; init; }
        public string? Title { get; init; }

        public GetUserInteractionQuery(string userId, int movieId, string? title, MediaType mediaType)
        {
            UserId = userId;
            MovieId = movieId;
            Title = title;
            MediaType = mediaType;
        }

        public class GetUserInteractionQueryHandler : IRequestHandler<GetUserInteractionQuery, Result<UserMovieInteractionDto>>
        {
            private readonly IUserInteractionService _interactionService;

            public GetUserInteractionQueryHandler(IUserInteractionService interactionService)
            {
                _interactionService = interactionService;
            }

            public async Task<Result<UserMovieInteractionDto>> Handle(GetUserInteractionQuery request, CancellationToken cancellationToken)
            {
                var result = await _interactionService.GetUserInteractionAsync(request.MediaType, request.UserId, request.MovieId, request.Title, cancellationToken);
                return result;
            }
        }
    }
}