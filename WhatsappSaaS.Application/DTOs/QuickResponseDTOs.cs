using System;

namespace WhatsappSaaS.Application.DTOs;

public class QuickResponseDto
{
    public Guid Id { get; set; }
    public string Shortcut { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public Guid CompanyId { get; set; }
}

public class CreateQuickResponseRequest
{
    public string Shortcut { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
}

public class UpdateQuickResponseRequest
{
    public string Shortcut { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
}
