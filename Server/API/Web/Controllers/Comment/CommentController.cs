namespace Web.Controllers.Comment
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Authorization;

    using Application.Interfaces;
    using Application.Handlers.Comments.Queries;
    using Application.Handlers.Comments.Commands;

    using Web.Extensions;

    public class CommentController : ApiController
    {
        private readonly IUser _currentUser;

        public CommentController(IUser currentUser)
        {
            _currentUser = currentUser;
        }

        [HttpGet("{movieId}")]
        public async Task<IActionResult> GetMovieComments(string movieId)
        {
            return await Mediator.Send(new GetMovieCommentsQuery { MovieId = movieId }).ToActionResult();
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> AddComment([FromBody] AddCommentCommand command)
        {
            return await Mediator.Send(command).ToActionResult();
        }

        [Authorize]
        [HttpDelete("{commentId}")]
        public async Task<IActionResult> DeleteComment(string commentId)
        {
            return await Mediator.Send(new DeleteCommentCommand { CommentId = commentId, UserId = _currentUser.Id }).ToActionResult();
        }
    }
}
