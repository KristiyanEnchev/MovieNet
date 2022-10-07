namespace Domain.Entities
{
    public class Comment : BaseAuditableEntity
    {
        public string Content { get; private set; }
        public string UserId { get; private set; }
        public User User { get; private set; }
        public int MovieId { get; private set; }
        public Movie Movie { get; private set; }

        private Comment() { }

        public Comment(string content, string userId, int movieId)
        {
            Id = Guid.NewGuid().ToString();
            Content = content;
            UserId = userId;
            MovieId = movieId;
        }

        public void UpdateContent(string newContent)
        {
            Content = newContent;
        }
    }
}