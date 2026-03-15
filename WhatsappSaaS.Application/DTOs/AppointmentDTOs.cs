using System;

namespace WhatsappSaaS.Application.DTOs;

public class AppointmentDto
{
    public Guid Id { get; set; }
    public Guid ContactId { get; set; }
    public string ContactName { get; set; } = string.Empty;
    public DateTime AppointmentDate { get; set; }
    public string Type { get; set; } = string.Empty;
    public string Notes { get; set; } = string.Empty;
    public Guid CompanyId { get; set; }
}

public class CreateAppointmentRequest
{
    public Guid ContactId { get; set; }
    public DateTime AppointmentDate { get; set; }
    public string Type { get; set; } = "Support";
    public string Notes { get; set; } = string.Empty;
}

public class UpdateAppointmentRequest
{
    public DateTime AppointmentDate { get; set; }
    public string Type { get; set; } = string.Empty;
    public string Notes { get; set; } = string.Empty;
}
