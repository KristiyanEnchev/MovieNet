﻿namespace Domain.Entities
{
    using MediatR;

    public abstract class BaseEvent : INotification
    {
        public DateTime DateOccurred { get; protected set; } = DateTime.UtcNow;
    }
}