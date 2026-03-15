using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WhatsappSaaS.Application.DTOs;
using WhatsappSaaS.Application.Common.Interfaces;
using WhatsappSaaS.Infrastructure.Persistence;

namespace WhatsappSaaS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DashboardController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly ITenantService _tenantService;

    public DashboardController(ApplicationDbContext context, ITenantService tenantService)
    {
        _context = context;
        _tenantService = tenantService;
    }

    [HttpGet("metrics")]
    public async Task<ActionResult<DashboardMetricsDto>> GetMetrics()
    {
        var companyId = _tenantService.CompanyId;

        var totalMessages = await _context.Messages.CountAsync();
        var totalLeads = await _context.Contacts.CountAsync();
        var activeConversations = await _context.Conversations.CountAsync(c => c.Status == Domain.Entities.ConversationStatus.InProgress);
        var finishedConversations = await _context.Conversations.CountAsync(c => c.Status == Domain.Entities.ConversationStatus.Finished);
        var newConversations = await _context.Conversations.CountAsync(c => c.Status == Domain.Entities.ConversationStatus.New);

        var attendantPerformance = await _context.Conversations
            .Include(c => c.AssignedUser)
            .Include(c => c.Messages)
            .Where(c => c.AssignedUserId != null)
            .GroupBy(c => new { c.AssignedUserId, c.AssignedUser!.Name })
            .Select(g => new AttendantPerformanceDto
            {
                AttendantName = g.Key.Name,
                ConversationsHandled = g.Count(),
                MessagesExchanged = g.Sum(c => c.Messages.Count)
            })
            .ToListAsync();

        var conversationVolume = await _context.Conversations
            .GroupBy(c => c.CreatedAt.Date)
            .OrderByDescending(g => g.Key)
            .Take(7)
            .Select(g => new ConversationVolumeDto
            {
                Period = g.Key.ToString("dd/MM"),
                Count = g.Count()
            })
            .ToListAsync();

        return Ok(new DashboardMetricsDto
        {
            TotalMessages = totalMessages,
            TotalLeads = totalLeads,
            ActiveConversations = activeConversations,
            FinishedConversations = finishedConversations,
            NewConversations = newConversations,
            AverageResponseTimeMinutes = 5.0, // Simplified for MVP
            AttendantPerformance = attendantPerformance,
            ConversationVolume = conversationVolume
        });
    }
}
