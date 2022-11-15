namespace Application.Handlers.UserInteractions.Queries
{
    using MediatR;

    using Application.Interfaces;

    using Models.Movie;

    using Shared;

    public record GetUserWatchlistQuery : IRequest<Result<PaginatedResult<MovieDto>>>
    {
        public string UserId { get; init; }
        public int PageNumber { get; init; } = 1;
        public int PageSize { get; init; } = 20;

        public GetUserWatchlistQuery(string userId, int pageNumber = 1, int pageSize = 20)
        {
            UserId = userId;
            PageNumber = pageNumber;
            PageSize = pageSize;
        }

        public class GetUserWatchlistQueryHandler : IRequestHandler<GetUserWatchlistQuery, Result<PaginatedResult<MovieDto>>>
        {
            private readonly IUserInteractionService _interactionService;

            public GetUserWatchlistQueryHandler(IUserInteractionService interactionService)
            {
                _interactionService = interactionService;
            }

            public async Task<Result<PaginatedResult<MovieDto>>> Handle(GetUserWatchlistQuery request, CancellationToken cancellationToken)
            {
                var result = await _interactionService.GetUserWatchlistAsync(request.UserId, request.PageNumber, request.PageSize, cancellationToken);
                return result;
            }
        }
    }
}