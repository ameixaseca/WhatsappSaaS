using System;
using System.Collections.Generic;

namespace WhatsappSaaS.Domain.Entities;

public enum ConversationStatus
{
    New,
    InProgress,
    Finished
}

public class Conversation : MultiTenantEntity
{
    public Guid ContactId { get; set; }
    public Contact Contact { get; set; } = null!;
    public Guid? AssignedUserId { get; set; }
    public User? AssignedUser { get; set; }
    public ConversationStatus Status { get; set; } = ConversationStatus.New;
    public ICollection<Message> Messages { get; set; } = new List<Message>();
}

public class Message : MultiTenantEntity
{
    public Guid ConversationId { get; set; }
    public Conversation Conversation { get; set; } = null!;
    public string Content { get; set; } = string.Empty;
    public bool IsFromContact { get; set; }
    public DateTime SentAt { get; set; } = DateTime.UtcNow;
}
