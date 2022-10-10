namespace Models
{
    public class CacheOptions
    {
        public string InstanceName { get; set; } = "MovieNet_";
        public TimeSpan DefaultExpiry { get; set; } = TimeSpan.FromHours(1);
        public bool EnableLogging { get; set; } = true;
    }
}