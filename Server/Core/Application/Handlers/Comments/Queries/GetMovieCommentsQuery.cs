namespace Application.Handlers.Comments.Queries
{
    using Application.Interfaces;

    using MediatR;

    using Models.Comments;

    using Shared;

    public class GetMovieCommentsQuery : IRequest<Result<PaginatedResult<CommentDto>>>
    {
        public string MovieId { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }

    public class GetMovieCommentsQueryHandler : IRequestHandler<GetMovieCommentsQuery, Result<PaginatedResult<CommentDto>>>
    {
        private readonly IUserInteractionService _userInteractionService;

        public GetMovieCommentsQueryHandler(IUserInteractionService userInteractionService)
        {
            _userInteractionService = userInteractionService;
        }

        public async Task<Result<PaginatedResult<CommentDto>>> Handle(GetMovieCommentsQuery request, CancellationToken cancellationToken)
        {

            return await _userInteractionService.GetMovieCommentsAsync(int.Parse(request.MovieId), request.Page, request.PageSize, cancellationToken);
        }
    }
}