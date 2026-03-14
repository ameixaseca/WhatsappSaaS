using System;
using System.Collections.Generic;

namespace WhatsappSaaS.Domain.Entities;

public class Tag : MultiTenantEntity
{
    public string Name { get; set; } = string.Empty;
    public string Color { get; set; } = string.Empty;
    public Company Company { get; set; } = null!;
    public ICollection<Contact> Contacts { get; set; } = new List<Contact>();
}
