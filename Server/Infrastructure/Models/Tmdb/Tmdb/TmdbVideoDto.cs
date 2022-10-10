namespace Models.Tmdb
{
    using System.Text.Json.Serialization;

    public class TmdbVideoDto
    {
        [JsonPropertyName("id")]
        public string TmdbId { get; set; }

        [JsonPropertyName("key")]
        public string Key { get; set; }

        [JsonPropertyName("site")]
        public string Site { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        public string Url { get; set; }
    }
}