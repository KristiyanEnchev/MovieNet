namespace Web.Controllers.Image
{
    using Microsoft.AspNetCore.Mvc;

    using Application.Handlers.Image.Query;

    public class ImageController : ApiController
    {
        private const int CACHE_DAYS = 7;

        [HttpGet("{type}/{imageId}")]
        public async Task<IActionResult> GetImage(
            [FromRoute] string type,
            [FromRoute] string imageId,
            CancellationToken cancellationToken = default)
        {
            var result = await Mediator.Send(new GetImageQuery(imageId, type), cancellationToken);

            if (!result.Success)
            {
                return NotFound();
            }

            var cacheDuration = CACHE_DAYS * 24 * 60 * 60;
            var expirationDate = DateTimeOffset.UtcNow.AddDays(CACHE_DAYS);

            Response.Headers["Cache-Control"] = $"public, max-age={cacheDuration}";
            Response.Headers["Expires"] = expirationDate.ToString("R");
            Response.Headers["ETag"] = $"W/\"{imageId}-{type}\"";

            return File(result.Data, "image/jpeg");
        }

        [HttpGet("{type}/{imageId}/base64")]
        public async Task<IActionResult> GetImageBase64(
            [FromRoute] string type,
            [FromRoute] string imageId,
            CancellationToken cancellationToken = default)
        {
            var result = await Mediator.Send(new GetBase64ImageQuery(imageId, type), cancellationToken);

            if (!result.Success)
            {
                return NotFound();
            }

            var cacheDuration = CACHE_DAYS * 24 * 60 * 60;
            Response.Headers["Cache-Control"] = $"public, max-age={cacheDuration}";

            return Ok(new { data = result.Data });
        }

        [HttpHead("{type}/{imageId}")]
        public async Task<IActionResult> CheckImage(
            [FromRoute] string type,
            [FromRoute] string imageId,
            CancellationToken cancellationToken = default)
        {
            var result = await Mediator.Send(new CheckImageExistsQuery(imageId, type), cancellationToken);

            if (!result.Success)
            {
                return StatusCode(500);
            }

            return result.Data ? Ok() : NotFound();
        }
    }
}
