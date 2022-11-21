namespace Web.Controllers.Movie
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Authorization;

    using Swashbuckle.AspNetCore.Annotations;

    using Application.Interfaces;
    using Application.Handlers.UserInteractions.Commands;
    using Application.Handlers.UserInteractions.Queries;

    using Shared;

    using Web.Extensions;

    using Models.Movie;

    using Domain.Enums;

    [Authorize]
    public class UserInteractionsController : ApiController
    {
        private readonly IUser _currentUser;

        public UserInteractionsController(IUser currentUser)
        {
            _currentUser = currentUser;
        }

        /// <summary>
        /// Toggle like status for a movie or TV show
        /// </summary>
        [HttpPost($"movies{PathSeparator}{Id}{PathSeparator}like")]
        [SwaggerOperation("Toggle like status for a specific media.")]
        [SwaggerResponse(200, "Like status toggled successfully", typeof(Result<bool>))]
        [SwaggerResponse(400, "Invalid movie ID")]
        [SwaggerResponse(401, "Unauthorized access")]
        [SwaggerResponse(500, "Internal server error")]
        public async Task<ActionResult<bool>> ToggleLike(
            [FromQuery] MediaType mediaType,
            int id,
            CancellationToken cancellationToken = default)
        {
            var command = new ToggleLikeCommand(_currentUser.Id, id, null, mediaType);
            return await Mediator.Send(command, cancellationToken).ToActionResult();
        }

        /// <summary>
        /// Toggle watchlist status for a movie or TV show
        /// </summary>
        [HttpPost($"watchlist")]
        [SwaggerOperation("Toggle watchlist status for a specific media.")]
        [SwaggerResponse(200, "Watchlist status toggled successfully", typeof(Result<bool>))]
        [SwaggerResponse(400, "Invalid movie ID")]
        [SwaggerResponse(401, "Unauthorized access")]
        [SwaggerResponse(500, "Internal server error")]
        public async Task<ActionResult<bool>> ToggleWatchlist(
            [FromBody] ToggleWatchlistRequest request,
            CancellationToken cancellationToken = default)
        {
            var command = new ToggleWatchlistCommand(_currentUser.Id, request.Id, request.Title, request.MediaType);
            return await Mediator.Send(command, cancellationToken).ToActionResult();
        }

        /// <summary>
        /// Get user's watchlist
        /// </summary>
        [HttpGet("watchlist")]
        [SwaggerOperation("Get the current user's watchlist with pagination.")]
        [SwaggerResponse(200, "Returns watchlist successfully", typeof(Result<PaginatedResult<MovieDto>>))]
        [SwaggerResponse(400, "Invalid pagination parameters")]
        [SwaggerResponse(401, "Unauthorized access")]
        [SwaggerResponse(500, "Internal server error")]
        public async Task<ActionResult<PaginatedResult<MovieDto>>> GetWatchlist(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20,
            CancellationToken cancellationToken = default)
        {
            var query = new GetUserWatchlistQuery(_currentUser.Id, page, pageSize);
            return await Mediator.Send(query, cancellationToken).ToActionResult();
        }

        /// <summary>
        /// Get user's interaction with a specific movie or TV show
        /// </summary>
        [HttpGet($"movies{PathSeparator}{Id}")]
        [SwaggerOperation("Get the current user's interaction details for a specific media item.")]
        [SwaggerResponse(200, "Returns user interaction successfully", typeof(Result<UserMovieInteractionDto>))]
        [SwaggerResponse(400, "Invalid movie ID")]
        [SwaggerResponse(401, "Unauthorized access")]
        [SwaggerResponse(404, "Interaction not found")]
        [SwaggerResponse(500, "Internal server error")]
        public async Task<ActionResult<UserMovieInteractionDto>> GetUserInteraction(
            [FromQuery] MediaType mediaType,
            int id,
            CancellationToken cancellationToken = default)
        {
            var query = new GetUserInteractionQuery(_currentUser.Id, id, null, mediaType);
            return await Mediator.Send(query, cancellationToken).ToActionResult();
        }
    }
}
