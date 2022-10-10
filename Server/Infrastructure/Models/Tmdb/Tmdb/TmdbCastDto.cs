namespace Models.Tmdb
{
    using System.Text.Json.Serialization;

    public class TmdbCastDto
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("character")]
        public string Character { get; set; }

        [JsonPropertyName("profile_path")]
        public string ProfilePath { get; set; }

        [JsonPropertyName("order")]
        public int Order { get; set; }
    }
}