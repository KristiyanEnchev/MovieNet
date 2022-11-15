namespace Application.Handlers.UserInteractions.Commands
{
    using MediatR;

    using Application.Interfaces;

    using Shared;

    using Domain.Enums;

    public record ToggleLikeCommand : IRequest<Result<bool>>
    {
        public MediaType MediaType { get; init; }
        public string UserId { get; init; }
        public int MovieId { get; init; }
        public string? Title { get; init; }

        public ToggleLikeCommand(string userId, int movieId, string? title, MediaType mediaType)
        {
            UserId = userId;
            MovieId = movieId;
            Title = title;
            MediaType = mediaType;
        }

        public class ToggleLikeCommandHandler : IRequestHandler<ToggleLikeCommand, Result<bool>>
        {
            private readonly IUserInteractionService _interactionService;

            public ToggleLikeCommandHandler(IUserInteractionService interactionService)
            {
                _interactionService = interactionService;
            }

            public async Task<Result<bool>> Handle(ToggleLikeCommand request, CancellationToken cancellationToken)
            {
                var result = await _interactionService.ToggleLikeAsync(request.MediaType, request.UserId, request.MovieId, request.Title, cancellationToken);
                return result;
            }
        }
    }
}