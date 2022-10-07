namespace Domain.Entities
{
    using Domain.Enums;

    public class Genre : BaseAuditableEntity
    {
        public int TmdbId { get; private set; }
        public string Name { get; private set; }
        public MediaType MediaType { get; private set; }
        public ICollection<Movie> Movies { get; private set; } = new List<Movie>();

        private Genre() { }

        public Genre(int tmdbId, string name, MediaType mediaType)
        {
            Id = Guid.NewGuid().ToString();
            TmdbId = tmdbId;
            Name = name;
            MediaType = mediaType;
        }

        public void UpdateDetails(string name, MediaType mediaType)
        {
            Name = name;
            MediaType = mediaType;
            UpdatedDate = DateTime.UtcNow;
        }

        public void UpdateName(string name)
        {
            Name = name;
            UpdatedDate = DateTime.UtcNow;
        }
    }
}