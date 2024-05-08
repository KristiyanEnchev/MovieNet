namespace Infrastructure.Services.Image
{
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Caching.Distributed;

    using Application.Interfaces;

    public class ImageCacheService : IImageCacheService
    {
        private readonly IDistributedCache _cache;
        private readonly HttpClient _httpClient;
        private readonly ILogger<ImageCacheService> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly DistributedCacheEntryOptions _defaultCacheOptions;
        private static readonly SemaphoreSlim _throttler = new(3);
        private const int CACHE_DAYS = 7;

        private static readonly HashSet<string> _validSizes = new(StringComparer.OrdinalIgnoreCase)
        {
            "original", "w500"
        };

        private static readonly Dictionary<string, string> _imageTypes = new(StringComparer.OrdinalIgnoreCase)
        {
            { "poster", "w500" },
            { "backdrop", "original" }
        };

        public ImageCacheService(
            IDistributedCache cache,
            HttpClient httpClient,
            ILogger<ImageCacheService> logger,
            IHttpContextAccessor httpContextAccessor)
        {
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));

            _defaultCacheOptions = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(CACHE_DAYS),
                SlidingExpiration = TimeSpan.FromDays(1)
            };
        }

        public string GetLocalImageUrl(string path, string imageType = "poster")
        {
            if (string.IsNullOrEmpty(path))
                return null;

            var imagePath = ExtractImagePath(path);
            if (string.IsNullOrEmpty(imagePath))
                return null;

            var request = _httpContextAccessor.HttpContext.Request;
            var baseUrl = $"{request.Scheme}://{request.Host}";
            return $"{baseUrl}/api/image/{imageType}/{imagePath}";
        }

        public async Task<byte[]> GetCachedImageAsync(
            string path,
            string imageType = "poster",
            bool bypassCache = false,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(path))
                return null;

            var imagePath = ExtractImagePath(path);
            if (string.IsNullOrEmpty(imagePath))
                return null;

            if (!_imageTypes.TryGetValue(imageType, out var size))
            {
                throw new ArgumentException($"Invalid image type. Valid values are: {string.Join(", ", _imageTypes.Keys)}", nameof(imageType));
            }

            if (bypassCache)
            {
                return await FetchImageAsync(imagePath, size, cancellationToken);
            }

            var cacheKey = GetCacheKey(imagePath, imageType);
            var cachedImage = await _cache.GetAsync(cacheKey, cancellationToken);

            if (cachedImage != null)
            {
                _logger.LogInformation("Image cache hit for path {Path}", path);
                return cachedImage;
            }

            var imageBytes = await FetchImageAsync(imagePath, size, cancellationToken);

            if (imageBytes != null)
            {
                await _cache.SetAsync(cacheKey, imageBytes, _defaultCacheOptions, cancellationToken);
                _logger.LogInformation("Cached image for path {Path}", path);
            }

            return imageBytes;
        }

        private async Task<byte[]> FetchImageAsync(string imagePath, string size, CancellationToken cancellationToken)
        {
            await _throttler.WaitAsync(cancellationToken);

            try
            {
                var tmdbUrl = $"https://image.tmdb.org/t/p/{size}/{imagePath}";
                using var response = await _httpClient.GetAsync(tmdbUrl, cancellationToken);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("Failed to fetch image for path {Path}. Status: {Status}",
                        imagePath, response.StatusCode);
                    return null;
                }

                return await response.Content.ReadAsByteArrayAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching image for path {Path}", imagePath);
                return null;
            }
            finally
            {
                _throttler.Release();
            }
        }

        private static string GetCacheKey(string imagePath, string imageType)
        {
            return $"tmdb:{imageType}:{imagePath}";
        }

        private static string ExtractImagePath(string url)
        {
            if (string.IsNullOrEmpty(url))
                return null;

            return url.Split('/').LastOrDefault();
        }
    }
}