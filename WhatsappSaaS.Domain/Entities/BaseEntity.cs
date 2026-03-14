using System;

namespace WhatsappSaaS.Domain.Entities;

public abstract class BaseEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}

public abstract class MultiTenantEntity : BaseEntity
{
    public Guid CompanyId { get; set; }
}
