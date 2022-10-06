namespace Domain.Interfaces
{
    using Domain.Entities;

    public interface IDomainEventDispatcher
    {
        Task DispatchAndClearEvents(IEnumerable<BaseAuditableEntity> entitiesWithEvents);
    }
}