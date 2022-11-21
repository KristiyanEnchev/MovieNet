namespace Web.Controllers.Movie
{
    using Microsoft.AspNetCore.Mvc;

    using Swashbuckle.AspNetCore.Annotations;

    using Application.Handlers.Genres.Queries;

    using Models.Movie;

    using Shared;

    using Domain.Enums;

    using Web.Extensions;

    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class GenresController : ApiController
    {
        /// <summary>
        /// Get genres for movies or TV shows
        /// </summary>
        [HttpGet]
        [SwaggerOperation("Get list of genres for movies or TV shows.")]
        [SwaggerResponse(200, "Returns genres successfully", typeof(Result<List<GenreDto>>))]
        [SwaggerResponse(400, "Invalid media type")]
        [SwaggerResponse(500, "Internal server error")]
        public async Task<ActionResult<List<GenreDto>>> GetGenres(
            [FromQuery] MediaType mediaType,
            [FromQuery] bool forceRefresh = false,
            CancellationToken cancellationToken = default)
        {
            var query = new GetGenresQuery(mediaType, forceRefresh);
            return await Mediator.Send(query, cancellationToken).ToActionResult();
        }
    }
}