namespace Domain.Entities
{
    using System.ComponentModel.DataAnnotations.Schema;

    using Microsoft.AspNetCore.Identity;

    using Domain.Interfaces;
    using Domain.Enums;

    public class User : IdentityUser, IAuditableEntity
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public bool IsActive { get; set; }
        public DateTime RefreshTokenExpiryTime { get; set; }

        public string? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }

        private readonly List<UserMovieInteraction> _movieInteractions = new();
        public IReadOnlyCollection<UserMovieInteraction> MovieInteractions => _movieInteractions.AsReadOnly();

        private readonly List<BaseEvent> _domainEvents = new();
        [NotMapped]
        public IReadOnlyCollection<BaseEvent> DomainEvents => _domainEvents.AsReadOnly();

        public void AddDomainEvent(BaseEvent domainEvent) => _domainEvents.Add(domainEvent);
        public void RemoveDomainEvent(BaseEvent domainEvent) => _domainEvents.Remove(domainEvent);
        public void ClearDomainEvents() => _domainEvents.Clear();

        public UserMovieInteraction AddMovieInteraction(int tmdbId, MediaType mediaType)
        {
            var interaction = new UserMovieInteraction(Id, tmdbId, mediaType);
            _movieInteractions.Add(interaction);
            return interaction;
        }
    }
}