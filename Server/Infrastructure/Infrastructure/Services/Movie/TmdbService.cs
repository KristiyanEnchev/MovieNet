namespace Infrastructure.Services.Movie
{
    using System.Net.Http.Json;
    using System.Net.Http.Headers;

    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;

    using Newtonsoft.Json.Linq;

    using Application.Interfaces;

    using Domain.Enums;

    using Infrastructure.Services.Helpers;

    using Models;
    using Models.Tmdb;
    using Models.Tmdb.Enums;

    using Shared;

    public class TmdbService : ITmdbService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<TmdbService> _logger;
        private readonly string _apiKey;
        private readonly string _imageBaseUrl;
        private readonly SemaphoreSlim _rateLimiter;

        public TmdbService(
            HttpClient httpClient,
            IOptions<TmdbOptions> options,
            ILogger<TmdbService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
            _apiKey = options.Value.ApiKey;
            _imageBaseUrl = options.Value.ImageBaseUrl;

            var requestsPerSecond = options.Value.RequestsPerSecondLimit;
            _rateLimiter = new SemaphoreSlim(requestsPerSecond, requestsPerSecond);

            _httpClient.BaseAddress = new Uri(options.Value.BaseUrl);
            _httpClient.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public async Task<Result<PaginatedResult<TmdbMovieDto>>> FetchTrendingAsync(
            MediaType mediaType,
            TimeWindow timeWindow = TimeWindow.day,
            CancellationToken cancellationToken = default)
        {
            try
            {
                await _rateLimiter.WaitAsync(cancellationToken);

                var response = await _httpClient.GetAsync($"/3/trending/{mediaType}/{timeWindow}?api_key={_apiKey}");
                response.EnsureSuccessStatusCode();

                var result = await response.Content.ReadFromJsonAsync<TmdbPagedResponse<TmdbMovieDto>>(cancellationToken: cancellationToken);

                if (result == null)
                {
                    return Result<PaginatedResult<TmdbMovieDto>>.Failure("Failed to deserialize TMDB response");
                }

                foreach (var item in result.Results)
                {
                    EnrichImageUrls(item);
                }

                return Result<PaginatedResult<TmdbMovieDto>>.SuccessResult(PaginationTransformer.TransformToPaginatedResult(result!));
            }

            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting trending {TimeWindow}", timeWindow);
                return Result<PaginatedResult<TmdbMovieDto>>.Failure($"TMDB API error: {ex.Message}");
            }
            finally
            {
                await ReleaseRateLimiter();
            }
        }

        public async Task<Result<TmdbMovieDetailsDto>> GetDetailsAsync(
           MediaType mediaType,
           int tmdbId,
           bool appendToResponse = false,
           CancellationToken cancellationToken = default)
        {
            try
            {
                await _rateLimiter.WaitAsync(cancellationToken);

                var uri = $"/3/{mediaType}/{tmdbId}?api_key={_apiKey}";
                if (appendToResponse)
                {
                    var appendResponses = new[] { "credits", "images", "videos", "keywords", "external_ids" };
                    var appendResponse = string.Join(",", appendResponses);

                    uri = $"{uri}&append_to_response={appendResponse}";
                }

                var response = await _httpClient.GetAsync(uri, cancellationToken);

                response.EnsureSuccessStatusCode();

                var result = await response.Content.ReadAsStringAsync(cancellationToken);
                var jsonObject = JObject.Parse(result);

                //var movieDetails = JsonConvert.DeserializeObject<TmdbMovieDetailsDto>(result);
                var movieDetails = await response.Content.ReadFromJsonAsync<TmdbMovieDetailsDto>(cancellationToken: cancellationToken);

                if (movieDetails == null)
                {
                    return Result<TmdbMovieDetailsDto>.Failure("Failed to deserialize TMDB response");
                }

                if (appendToResponse)
                {
                    ProcessDetailsResponse(movieDetails, jsonObject);
                }

                EnrichImageUrls(movieDetails);

                return Result<TmdbMovieDetailsDto>.SuccessResult(movieDetails);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting {MediaType} details. TmdbId: {TmdbId}", mediaType, tmdbId);
                return Result<TmdbMovieDetailsDto>.Failure($"TMDB API error: {ex.Message}");
            }
            finally
            {
                await ReleaseRateLimiter();
            }
        }

        public async Task<Result<PaginatedResult<TmdbMovieDto>>> SearchAsync(
            string query,
            MediaType mediatype = MediaType.multi,
            int page = 1,
            CancellationToken cancellationToken = default)
        {
            try
            {
                await _rateLimiter.WaitAsync(cancellationToken);

                var queryParams = new List<string>
                {
                    $"api_key={_apiKey}",
                    $"query={Uri.EscapeDataString(query)}",
                    $"page={page}"
                };

                var response = await _httpClient.GetAsync(
                    $"/3/search/{mediatype}?{string.Join("&", queryParams)}",
                    cancellationToken);

                response.EnsureSuccessStatusCode();

                var result = await response.Content.ReadFromJsonAsync<TmdbPagedResponse<TmdbMovieDto>>(
                    cancellationToken: cancellationToken);

                if (result == null)
                    return Result<PaginatedResult<TmdbMovieDto>>.Failure("Failed to deserialize TMDB response");

                foreach (var item in result.Results)
                {
                    EnrichImageUrls(item);
                }

                return Result<PaginatedResult<TmdbMovieDto>>.SuccessResult(PaginationTransformer.TransformToPaginatedResult(result));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching. Query: {Query}", query);
                return Result<PaginatedResult<TmdbMovieDto>>.Failure($"TMDB API error: {ex.Message}");
            }
            finally
            {
                await ReleaseRateLimiter();
            }
        }

        public async Task<Result<PaginatedResult<TmdbMovieDto>>> DiscoverAsync(
            MediaType mediaType = MediaType.movie,
            SortingOptions sortBy = SortingOptions.popularity_desc,
            int[] withGenres = null,
            string year = null,
            int page = 1,
            CancellationToken cancellationToken = default)
        {
            try
            {
                if (!string.IsNullOrEmpty(year) && !IsValidYear(year))
                {
                    return Result<PaginatedResult<TmdbMovieDto>>.Failure($"Invalid year: {year}. Year must be a numeric value between 1900 and 2100.");
                }

                await _rateLimiter.WaitAsync(cancellationToken);
                var sort = ToSortQuery(sortBy);

                var queryParams = new List<string>
                {
                    $"api_key={_apiKey}",
                    $"page={page}",
                    $"sort_by={sort}"
                };

                if (withGenres != null && withGenres.Length > 0)
                    queryParams.Add($"with_genres={string.Join(",", withGenres)}");

                var response = await _httpClient.GetAsync(
                    $"/3/discover/{mediaType}?{string.Join("&", queryParams)}",
                    cancellationToken);

                response.EnsureSuccessStatusCode();

                var result = await response.Content.ReadFromJsonAsync<TmdbPagedResponse<TmdbMovieDto>>(
                    cancellationToken: cancellationToken);

                if (result == null)
                {
                    return Result<PaginatedResult<TmdbMovieDto>>.Failure("Failed to deserialize TMDB response");
                }

                foreach (var item in result.Results)
                {
                    EnrichImageUrls(item);
                    item.MediaType = mediaType;
                }

                return Result<PaginatedResult<TmdbMovieDto>>.SuccessResult(PaginationTransformer.TransformToPaginatedResult(result));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error discovering {MediaType}s", mediaType);
                return Result<PaginatedResult<TmdbMovieDto>>.Failure($"TMDB API error: {ex.Message}");
            }
            finally
            {
                await ReleaseRateLimiter();
            }
        }

        public async Task<Result<List<TmdbGenreDto>>> GetGenresAsync(
            MediaType mediaType,
            CancellationToken cancellationToken = default)
        {
            try
            {
                await _rateLimiter.WaitAsync(cancellationToken);

                var response = await _httpClient.GetAsync(
                    $"/3/genre/{mediaType}/list?api_key={_apiKey}",
                    cancellationToken);

                response.EnsureSuccessStatusCode();

                var result = await response.Content.ReadFromJsonAsync<GenreResponse>(
                    cancellationToken: cancellationToken);

                if (result == null)
                    return Result<List<TmdbGenreDto>>.Failure("Failed to deserialize TMDB response");

                return Result<List<TmdbGenreDto>>.SuccessResult(result.Genres);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching genres for {MediaType}", mediaType);
                return Result<List<TmdbGenreDto>>.Failure($"TMDB API error: {ex.Message}");
            }
            finally
            {
                await ReleaseRateLimiter();
            }
        }

        public async Task<Result<TmdbCreditsDto>> GetCreditsAsync(
            MediaType mediaType,
            int tmdbId,
            CancellationToken cancellationToken = default)
        {
            try
            {
                await _rateLimiter.WaitAsync(cancellationToken);

                var uri = $"/3/{mediaType}/{tmdbId}/credits?api_key={_apiKey}";
                var response = await _httpClient.GetAsync(uri, cancellationToken);
                response.EnsureSuccessStatusCode();

                var result = await response.Content.ReadFromJsonAsync<TmdbCreditsDto>(cancellationToken: cancellationToken);

                if (result == null)
                {
                    return Result<TmdbCreditsDto>.Failure("Failed to deserialize TMDB response");
                }

                return Result<TmdbCreditsDto>.SuccessResult(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting credits for {MediaType} with ID {TmdbId}", mediaType, tmdbId);
                return Result<TmdbCreditsDto>.Failure($"TMDB API error: {ex.Message}");
            }
            finally
            {
                await ReleaseRateLimiter();
            }
        }

       

        private void EnrichImageUrls(TmdbMovieDto movie)
        {
            if (!string.IsNullOrEmpty(movie.PosterPath))
                movie.PosterPath = GetFullImageUrl(movie.PosterPath, "w500");

            if (!string.IsNullOrEmpty(movie.BackdropPath))
                movie.BackdropPath = GetFullImageUrl(movie.BackdropPath, "original");
        }

        private string GetFullImageUrl(string path, string size)
        {
            if (string.IsNullOrEmpty(path)) return string.Empty;
            return $"{_imageBaseUrl}{size}{path}";
        }

        private async Task ReleaseRateLimiter()
        {
            await Task.Delay(1000 / _rateLimiter.CurrentCount);
            _rateLimiter.Release();
        }

        private string ToSortQuery(SortingOptions sortBy)
        {
            var sortString = sortBy.ToString().ToLower();
            int lastUnderscoreIndex = sortString.LastIndexOf('_');

            if (lastUnderscoreIndex >= 0)
            {
                sortString = sortString.Substring(0, lastUnderscoreIndex) + "." + sortString.Substring(lastUnderscoreIndex + 1);
            }

            return $"sort_by={sortString}";
        }

        private bool IsValidYear(string year)
        {
            if (int.TryParse(year, out var numericYear))
            {
                return numericYear >= 1900 && numericYear <= 2100;
            }
            return false;
        }

        private void ProcessDetailsResponse(TmdbMovieDetailsDto movieDetails, JObject jsonObject)
        {
            try
            {
                if (jsonObject["credits"] != null)
                {
                    if (jsonObject["credits"]["cast"] != null)
                    {
                        movieDetails.Cast = jsonObject["credits"]["cast"].ToObject<List<TmdbCastDto>>();
                    }
                }

                if (jsonObject["videos"] != null)
                {
                    movieDetails.Videos = new TmdbVideoResultsDto
                    {
                        Results = jsonObject["videos"]["results"]?.ToObject<List<TmdbVideoDto>>() ?? new List<TmdbVideoDto>()
                    };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing details response");
            }
        }
    }
}