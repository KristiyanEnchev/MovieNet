namespace Domain.Events
{
    using Domain.Entities;

    public class CommentDeletedEvent : BaseEvent
    {
        public string CommentId { get; set; }
        public string MovieId { get; set; }
        public string UserId { get; set; }
    }
}
