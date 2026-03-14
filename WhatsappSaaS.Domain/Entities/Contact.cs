using System;
using System.Collections.Generic;

namespace WhatsappSaaS.Domain.Entities;

public class Contact : MultiTenantEntity
{
    public string Name { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string Notes { get; set; } = string.Empty;
    public Company Company { get; set; } = null!;
    public ICollection<Tag> Tags { get; set; } = new List<Tag>();
    public ICollection<Conversation> Conversations { get; set; } = new List<Conversation>();
    public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
}
