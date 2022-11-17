namespace Application.Handlers.Comments.Commands
{
    using Application.Interfaces;

    using MediatR;

    using Shared;

    public class DeleteCommentCommand : IRequest<Result<string>>
    {
        public string CommentId { get; set; }
        public string MovieId { get; set; }
        public string UserId { get; set; }
    }

    public class DeleteCommentCommandHandler : IRequestHandler<DeleteCommentCommand, Result<string>>
    {
        private readonly IUserInteractionService _userInteractionService;

        public DeleteCommentCommandHandler(IUserInteractionService userInteractionService)
        {
            _userInteractionService = userInteractionService;
        }

        public async Task<Result<string>> Handle(DeleteCommentCommand request, CancellationToken cancellationToken)
        {
            return await _userInteractionService.DeleteCommentAsync(request.CommentId, int.Parse(request.MovieId), request.UserId, cancellationToken);
        }
    }
}
