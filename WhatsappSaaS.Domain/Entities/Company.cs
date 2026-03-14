using System;
using System.Collections.Generic;

namespace WhatsappSaaS.Domain.Entities;

public class Company : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Cnpj { get; set; } = string.Empty;
    public Guid PlanId { get; set; }
    public Plan Plan { get; set; } = null!;
    public bool IsActive { get; set; } = true;
    
    public ICollection<User> Users { get; set; } = new List<User>();
    public ICollection<Contact> Contacts { get; set; } = new List<Contact>();
    public ICollection<Tag> Tags { get; set; } = new List<Tag>();
}
