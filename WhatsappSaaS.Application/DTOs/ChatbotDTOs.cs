using System;
using System.Collections.Generic;

namespace WhatsappSaaS.Application.DTOs;

public class ChatbotFlowDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public Guid CompanyId { get; set; }
    public List<FlowStepDto> Steps { get; set; } = new();
}

public class FlowStepDto
{
    public Guid Id { get; set; }
    public string Content { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public int Order { get; set; }
    public string OptionsJson { get; set; } = string.Empty;
}

public class CreateChatbotFlowRequest
{
    public string Name { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public List<CreateFlowStepRequest> Steps { get; set; } = new();
}

public class CreateFlowStepRequest
{
    public string Content { get; set; } = string.Empty;
    public string Type { get; set; } = "Response";
    public int Order { get; set; }
    public string OptionsJson { get; set; } = string.Empty;
}
