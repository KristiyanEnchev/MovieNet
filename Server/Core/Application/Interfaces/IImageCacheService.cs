namespace Application.Interfaces
{
    public interface IImageCacheService
    {
        string GetLocalImageUrl(string path, string imageType = "poster");
        Task<byte[]> GetCachedImageAsync(string path, string imageType = "poster", bool bypassCache = false, CancellationToken cancellationToken = default);
    }
}