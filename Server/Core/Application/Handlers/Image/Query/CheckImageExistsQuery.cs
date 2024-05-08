namespace Application.Handlers.Image.Query
{
    using Microsoft.Extensions.Logging;

    using MediatR;

    using Shared;

    using Application.Interfaces;

    public record CheckImageExistsQuery(string ImagePath, string ImageType = "poster") : IRequest<Result<bool>>;

    public class CheckImageExistsQueryHandler : IRequestHandler<CheckImageExistsQuery, Result<bool>>
    {
        private readonly IImageCacheService _imageService;
        private readonly ILogger<CheckImageExistsQueryHandler> _logger;

        public CheckImageExistsQueryHandler(IImageCacheService imageService, ILogger<CheckImageExistsQueryHandler> logger)
        {
            _imageService = imageService;
            _logger = logger;
        }

        public async Task<Result<bool>> Handle(CheckImageExistsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var imageBytes = await _imageService.GetCachedImageAsync(
                    request.ImagePath,
                    request.ImageType,
                    false,
                    cancellationToken);

                return Result<bool>.SuccessResult(imageBytes != null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking image existence for path {ImagePath}", request.ImagePath);
                return Result<bool>.Failure($"Error checking image: {ex.Message}");
            }
        }
    }
}