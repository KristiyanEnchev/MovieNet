namespace Domain.Events
{
    using Domain.Entities;

    public class UserMovieInteractionChangedEvent : BaseEvent
    {
        public UserMovieInteraction Interaction { get; }

        public UserMovieInteractionChangedEvent(UserMovieInteraction interaction)
        {
            Interaction = interaction;
        }
    }
}