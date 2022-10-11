namespace Models.Movie
{
    public class VideoDto
    {
        public string TmdbId { get; set; }
        public string Name { get; set; }
        public string Key { get; set; }
        public string Site { get; set; }
        public int Size { get; set; }
        public string Type { get; set; }
        public bool Official { get; set; }
        public DateTime PublishedAt { get; set; }
    }
}
