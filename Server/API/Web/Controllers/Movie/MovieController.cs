namespace Web.Controllers.Movie
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Authorization;

    using Swashbuckle.AspNetCore.Annotations;

    using Application.Interfaces;
    using Application.Handlers.Movies.Queries;
    using Application.Handlers.Movies.Commands;

    using Domain.Enums;

    using Models.Movie;
    using Models.Tmdb;
    using Models.Tmdb.Enums;

    using Shared;

    using Web.Extensions;

    public class MoviesController : ApiController
    {
        private readonly IUser _currentUser;

        public MoviesController(IUser currentUser)
        {
            _currentUser = currentUser;
        }

        /// <summary>
        /// Get trending movies or TV shows
        /// </summary>
        [HttpGet("trending")]
        [SwaggerOperation("Gets trending movies or TV shows based on media type and time window.")]
        [SwaggerResponse(200, "Returns trending media list successfully", typeof(Result<PaginatedResult<MovieDto>>))]
        [SwaggerResponse(400, "Invalid parameters supplied")]
        [SwaggerResponse(500, "Internal server error")]
        public async Task<IActionResult> GetTrending(
            [FromQuery] MediaType mediaType,
            [FromQuery] TimeWindow timeWindow,
            CancellationToken cancellationToken = default)
        {
            var query = new GetTrendingMoviesQuery(mediaType, timeWindow, _currentUser.Id);
            return await Mediator.Send(query, cancellationToken).ToActionResult();
        }

        /// <summary>
        /// Get details for a specific movie or TV show
        /// </summary>
        [HttpGet($"{{tmdbId:int}}")]
        [SwaggerOperation("Gets detailed information for a specific movie or TV show.")]
        [SwaggerResponse(200, "Returns media details successfully", typeof(Result<MovieDetailsDto>))]
        [SwaggerResponse(400, "Invalid TMDB ID supplied")]
        [SwaggerResponse(404, "Media not found")]
        [SwaggerResponse(500, "Internal server error")]
        public async Task<ActionResult<MovieDetailsDto>> GetDetails(
            int tmdbId,
            [FromQuery] MediaType mediaType,
            CancellationToken cancellationToken = default)
        {
            var query = new GetMovieDetailsQuery(mediaType, tmdbId, _currentUser.Id);
            return await Mediator.Send(query, cancellationToken).ToActionResult();
        }

        /// <summary>
        /// Search for movies or TV shows
        /// </summary>
        [HttpGet("search")]
        [SwaggerOperation("Search for movies or TV shows with optional filters.")]
        [SwaggerResponse(200, "Returns search results successfully", typeof(Result<PaginatedResult<MovieDto>>))]
        [SwaggerResponse(400, "Invalid search parameters")]
        [SwaggerResponse(500, "Internal server error")]
        public async Task<ActionResult<PaginatedResult<MovieDto>>> Search(
            [FromQuery] MediaType mediaType,
            [FromQuery] string query,
            [FromQuery] int page = 1,
            CancellationToken cancellationToken = default)
        {
            var searchQuery = new SearchMoviesQuery(mediaType, query, page, _currentUser.Id);
            return await Mediator.Send(searchQuery, cancellationToken).ToActionResult();
        }

        /// <summary>
        /// Get all movies or TV shows with filtering options
        /// </summary>
        [HttpGet]
        [SwaggerOperation("Get all movies or TV shows with various filtering and sorting options.")]
        [SwaggerResponse(200, "Returns filtered media list successfully", typeof(Result<PaginatedResult<MovieDto>>))]
        [SwaggerResponse(400, "Invalid filter parameters")]
        [SwaggerResponse(500, "Internal server error")]
        public async Task<ActionResult<PaginatedResult<MovieDto>>> GetAll(
            [FromQuery] MediaType mediaType,
            [FromQuery] string? withGenres,
            [FromQuery] string? year,
            [FromQuery] SortingOptions sortBy = SortingOptions.popularity_desc,
            [FromQuery] int page = 1,
            CancellationToken cancellationToken = default)
        {
            var query = new GetAllMoviesQuery(mediaType, withGenres, page, _currentUser.Id, sortBy, year );
            return await Mediator.Send(query, cancellationToken).ToActionResult();
        }

        /// <summary>
        /// Sync movie or TV show data from TMDB
        /// </summary>
        [HttpPost($"{PathSeparator}{{tmdbId:int}}{PathSeparator}sync")]
        [Authorize(Roles = "Administrator")]
        [SwaggerOperation("Sync movie or TV show data from TMDB. Admin only.")]
        [SwaggerResponse(200, "Media synced successfully", typeof(Result<MovieDetailsDto>))]
        [SwaggerResponse(400, "Invalid TMDB ID")]
        [SwaggerResponse(401, "Unauthorized access")]
        [SwaggerResponse(403, "Forbidden - Admin access required")]
        [SwaggerResponse(500, "Internal server error")]
        public async Task<ActionResult<MovieDetailsDto>> SyncMovie(
            int tmdbId,
            [FromQuery] MediaType mediaType,
            CancellationToken cancellationToken = default)
        {
            var command = new SyncMovieCommand(mediaType, tmdbId);
            return await Mediator.Send(command, cancellationToken).ToActionResult();
        }

        /// <summary>
        /// Get credits (cast and crew) for a specific movie or TV show
        /// </summary>
        [HttpGet("{mediaType}/{id}/credits")]
        [SwaggerOperation("Get credits (cast and crew) for a specific media.")]
        [SwaggerResponse(200, "Credits retrieved successfully", typeof(Result<TmdbCreditsDto>))]
        [SwaggerResponse(400, "Invalid request or movie ID")]
        [SwaggerResponse(500, "Internal server error")]
        public async Task<ActionResult<Result<TmdbCreditsDto>>> GetCredits(
            [FromRoute] MediaType mediaType,
            [FromRoute] int id,
            CancellationToken cancellationToken)
        {
            var query = new GetMovieCreditsQuery(mediaType, id);
            return await Mediator.Send(query, cancellationToken).ToActionResult();
        }

        /// <summary>
        /// Get videos for a specific movie or TV show
        /// </summary>
        [HttpGet("{mediaType}/{id}/videos")]
        [SwaggerOperation("Get videos (trailers, teasers, etc.) for a specific media.")]
        [SwaggerResponse(200, "Videos retrieved successfully", typeof(Result<TmdbVideoResultsDto>))]
        [SwaggerResponse(400, "Invalid request or movie ID")]
        [SwaggerResponse(500, "Internal server error")]
        public async Task<ActionResult<Result<TmdbVideoResultsDto>>> GetVideos(
            [FromRoute] MediaType mediaType,
            [FromRoute] int id,
            CancellationToken cancellationToken)
        {
            var query = new GetMovieVideosQuery(mediaType, id);
            return await Mediator.Send(query, cancellationToken).ToActionResult();
        }
    }
}
