namespace Application.Handlers.Comments.Commands
{
    using Application.Interfaces;

    using MediatR;

    using Models.Comments;

    using Shared;

    public class AddCommentCommand : IRequest<Result<CommentDto>>
    {
        public string Content { get; set; }
        public string MovieId { get; set; }
        public string UserId { get; set; }
    }

    public class AddCommentCommandHandler : IRequestHandler<AddCommentCommand, Result<CommentDto>>
    {
        private readonly IUserInteractionService _userInteractionService;

        public AddCommentCommandHandler(IUserInteractionService userInteractionService)
        {
            _userInteractionService = userInteractionService;
        }
        public async Task<Result<CommentDto>> Handle(AddCommentCommand request, CancellationToken cancellationToken)
        {
            return await _userInteractionService.AddCommentAsync(request.UserId, int.Parse(request.MovieId), request.Content, cancellationToken);
        }
    }
}