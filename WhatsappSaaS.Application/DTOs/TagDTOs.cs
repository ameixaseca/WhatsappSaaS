using System;

namespace WhatsappSaaS.Application.DTOs;

public class TagDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Color { get; set; } = string.Empty;
    public Guid CompanyId { get; set; }
}

public class CreateTagRequest
{
    public string Name { get; set; } = string.Empty;
    public string Color { get; set; } = "#007bff";
}

public class UpdateTagRequest
{
    public string Name { get; set; } = string.Empty;
    public string Color { get; set; } = string.Empty;
}
