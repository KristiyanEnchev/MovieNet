namespace Application.Handlers.Image.Query
{
    using Microsoft.Extensions.Logging;

    using MediatR;

    using Shared;

    using Application.Interfaces;

    public record GetImageQuery(string ImagePath, string ImageType = "poster", bool BypassCache = false) : IRequest<Result<byte[]>>;

    public class GetImageQueryHandler : IRequestHandler<GetImageQuery, Result<byte[]>>
    {
        private readonly IImageCacheService _imageService;
        private readonly ILogger<GetImageQueryHandler> _logger;

        public GetImageQueryHandler(IImageCacheService imageService, ILogger<GetImageQueryHandler> logger)
        {
            _imageService = imageService;
            _logger = logger;
        }

        public async Task<Result<byte[]>> Handle(GetImageQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var imageBytes = await _imageService.GetCachedImageAsync(
                    request.ImagePath,
                    request.ImageType,
                    request.BypassCache,
                    cancellationToken);

                if (imageBytes == null)
                {
                    return Result<byte[]>.Failure("Image not found");
                }

                return Result<byte[]>.SuccessResult(imageBytes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting image for path {ImagePath}", request.ImagePath);
                return Result<byte[]>.Failure($"Error retrieving image: {ex.Message}");
            }
        }
    }
}