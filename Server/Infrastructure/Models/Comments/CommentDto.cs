namespace Models.Comments
{
    public class CommentDto
    {
        public string Id { get; set; }
        public string Content { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public int MovieId { get; set; }
        public DateTime Created { get; set; }
        public DateTime? Modified { get; set; }
    }
}
