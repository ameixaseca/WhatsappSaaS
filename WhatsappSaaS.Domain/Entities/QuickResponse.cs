using System;

namespace WhatsappSaaS.Domain.Entities;

public class QuickResponse : MultiTenantEntity
{
    public string Shortcut { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public Company Company { get; set; } = null!;
}
