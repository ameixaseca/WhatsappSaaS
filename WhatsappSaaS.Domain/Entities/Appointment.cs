using System;

namespace WhatsappSaaS.Domain.Entities;

public enum AppointmentType
{
    Support,
    Sales,
    Other
}

public class Appointment : MultiTenantEntity
{
    public Guid ContactId { get; set; }
    public Contact Contact { get; set; } = null!;
    public DateTime AppointmentDate { get; set; }
    public AppointmentType Type { get; set; }
    public string Notes { get; set; } = string.Empty;
}
