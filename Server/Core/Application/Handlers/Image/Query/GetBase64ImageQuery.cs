namespace Application.Handlers.Image.Query
{
    using Microsoft.Extensions.Logging;

    using MediatR;

    using Shared;

    using Application.Interfaces;

    public record GetBase64ImageQuery(string ImagePath, string ImageType = "poster") : IRequest<Result<string>>;

    public class GetBase64ImageQueryHandler : IRequestHandler<GetBase64ImageQuery, Result<string>>
    {
        private readonly IImageCacheService _imageService;
        private readonly ILogger<GetBase64ImageQueryHandler> _logger;

        public GetBase64ImageQueryHandler(IImageCacheService imageService, ILogger<GetBase64ImageQueryHandler> logger)
        {
            _imageService = imageService;
            _logger = logger;
        }

        public async Task<Result<string>> Handle(GetBase64ImageQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var imageBytes = await _imageService.GetCachedImageAsync(
                    request.ImagePath,
                    request.ImageType,
                    false,
                    cancellationToken);

                if (imageBytes == null)
                {
                    return Result<string>.Failure("Image not found");
                }

                var base64String = Convert.ToBase64String(imageBytes);
                return Result<string>.SuccessResult($"data:image/jpeg;base64,{base64String}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting base64 image for path {ImagePath}", request.ImagePath);
                return Result<string>.Failure($"Error retrieving base64 image: {ex.Message}");
            }
        }
    }
}