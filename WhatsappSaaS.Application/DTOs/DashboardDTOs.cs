using System;
using System.Collections.Generic;

namespace WhatsappSaaS.Application.DTOs;

public class DashboardMetricsDto
{
    public int TotalMessages { get; set; }
    public int TotalLeads { get; set; }
    public int ActiveConversations { get; set; }
    public int FinishedConversations { get; set; }
    public int NewConversations { get; set; }
    public double AverageResponseTimeMinutes { get; set; }
    public List<AttendantPerformanceDto> AttendantPerformance { get; set; } = new();
    public List<ConversationVolumeDto> ConversationVolume { get; set; } = new();
}

public class AttendantPerformanceDto
{
    public string AttendantName { get; set; } = string.Empty;
    public int ConversationsHandled { get; set; }
    public int MessagesExchanged { get; set; }
}

public class ConversationVolumeDto
{
    public string Period { get; set; } = string.Empty;
    public int Count { get; set; }
}
