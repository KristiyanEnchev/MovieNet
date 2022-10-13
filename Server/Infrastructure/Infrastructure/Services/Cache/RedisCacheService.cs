namespace Infrastructure.Services.Cache
{
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;

    using Application.Interfaces;

    using Models;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;

    using StackExchange.Redis;

    public class RedisCacheService : ICacheService
    {
        private readonly IConnectionMultiplexer _redis;
        private readonly ILogger<RedisCacheService> _logger;
        private readonly CacheOptions _options;
        private readonly JsonSerializerSettings _jsonSettings;

        public RedisCacheService(
            IConnectionMultiplexer redis,
            ILogger<RedisCacheService> logger,
            IOptions<CacheOptions> options)
        {
            _redis = redis ?? throw new ArgumentNullException(nameof(redis));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _options = options?.Value ?? new CacheOptions();

            _jsonSettings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                MissingMemberHandling = MissingMemberHandling.Ignore,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor,
                TypeNameHandling = TypeNameHandling.All,
                ContractResolver = new DefaultContractResolver
                {
                    IgnoreSerializableAttribute = true
                }
            };
        }

        public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default) where T : class
        {
            try
            {
                var db = _redis.GetDatabase();
                var fullKey = GetFullKey(key);
                var value = await db.StringGetAsync(fullKey);

                if (!value.HasValue)
                {
                    LogIfEnabled(() => _logger.LogDebug("Cache miss for key: {Key}", fullKey));
                    return default;
                }

                LogIfEnabled(() => _logger.LogDebug("Cache hit for key: {Key}", fullKey));
                return JsonConvert.DeserializeObject<T>(value!, _jsonSettings);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving cached value for key {Key}", key);
                return default;
            }
        }

        public async Task<T> GetOrSetAsync<T>(
            string key,
            Func<Task<T>> factory,
            TimeSpan? expiry = null,
            CancellationToken cancellationToken = default) where T : class
        {
            var cached = await GetAsync<T>(key, cancellationToken);
            if (cached != null) return cached;

            var value = await factory();
            await SetAsync(key, value, expiry, cancellationToken);
            return value;
        }

        public async Task SetAsync<T>(
            string key,
            T value,
            TimeSpan? expiry = null,
            CancellationToken cancellationToken = default) where T : class
        {
            try
            {
                var db = _redis.GetDatabase();
                var fullKey = GetFullKey(key);
                var serialized = JsonConvert.SerializeObject(value, _jsonSettings);

                await db.StringSetAsync(
                    fullKey,
                    serialized,
                    expiry ?? _options.DefaultExpiry
                );

                LogIfEnabled(() => _logger.LogDebug("Set cache for key: {Key}", fullKey));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error setting cached value for key {Key}", key);
            }
        }

        public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
        {
            try
            {
                var db = _redis.GetDatabase();
                var fullKey = GetFullKey(key);
                await db.KeyDeleteAsync(fullKey);

                LogIfEnabled(() => _logger.LogDebug("Removed cache for key: {Key}", fullKey));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing cached value for key {Key}", key);
            }
        }

        public async Task RemoveByPrefixAsync(string prefixKey, CancellationToken cancellationToken = default)
        {
            try
            {
                var prefix = GetFullKey(prefixKey);
                var endpoints = _redis.GetEndPoints();
                var keys = new List<RedisKey>();

                foreach (var endpoint in endpoints)
                {
                    var server = _redis.GetServer(endpoint);
                    keys.AddRange(server.Keys(pattern: $"{prefix}*"));
                }

                var db = _redis.GetDatabase();
                var tasks = keys.Select(key => db.KeyDeleteAsync(key));
                await Task.WhenAll(tasks);

                LogIfEnabled(() => _logger.LogDebug("Removed {Count} keys with prefix: {Prefix}", keys.Count, prefix));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing cached values with prefix {Prefix}", prefixKey);
            }
        }

        private string GetFullKey(string key) => $"{_options.InstanceName}{key}";

        private void LogIfEnabled(Action logAction)
        {
            if (_options.EnableLogging)
            {
                logAction();
            }
        }
    }
}