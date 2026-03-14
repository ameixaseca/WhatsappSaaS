using System;
using System.Collections.Generic;

namespace WhatsappSaaS.Domain.Entities;

public class ChatbotFlow : MultiTenantEntity
{
    public string Name { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public ICollection<FlowStep> Steps { get; set; } = new List<FlowStep>();
}

public enum FlowStepType
{
    Question,
    Response,
    Menu,
    TransferToHuman
}

public class FlowStep : MultiTenantEntity
{
    public Guid ChatbotFlowId { get; set; }
    public ChatbotFlow ChatbotFlow { get; set; } = null!;
    public string Content { get; set; } = string.Empty;
    public FlowStepType Type { get; set; }
    public int Order { get; set; }
    public string OptionsJson { get; set; } = string.Empty; // Store menu options as JSON
}
