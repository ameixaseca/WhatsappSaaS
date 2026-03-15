using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WhatsappSaaS.Application.DTOs;
using WhatsappSaaS.Application.Common.Interfaces;
using WhatsappSaaS.Domain.Entities;
using WhatsappSaaS.Infrastructure.Hubs;
using WhatsappSaaS.Infrastructure.Persistence;

namespace WhatsappSaaS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ConversationsController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly ITenantService _tenantService;
    private readonly IHubContext<ChatHub> _hubContext;

    public ConversationsController(ApplicationDbContext context, ITenantService tenantService, IHubContext<ChatHub> hubContext)
    {
        _context = context;
        _tenantService = tenantService;
        _hubContext = hubContext;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ConversationDto>>> GetAll([FromQuery] string? status = null)
    {
        var query = _context.Conversations
            .Include(c => c.Contact)
            .Include(c => c.AssignedUser)
            .Include(c => c.Messages)
            .AsQueryable();

        if (!string.IsNullOrEmpty(status) && Enum.TryParse<ConversationStatus>(status, out var parsedStatus))
        {
            query = query.Where(c => c.Status == parsedStatus);
        }

        var conversations = await query
            .OrderByDescending(c => c.UpdatedAt ?? c.CreatedAt)
            .Select(c => new ConversationDto
            {
                Id = c.Id,
                ContactId = c.ContactId,
                ContactName = c.Contact.Name,
                ContactPhone = c.Contact.PhoneNumber,
                AssignedUserId = c.AssignedUserId,
                AssignedUserName = c.AssignedUser != null ? c.AssignedUser.Name : null,
                Status = c.Status.ToString(),
                CreatedAt = c.CreatedAt,
                UpdatedAt = c.UpdatedAt,
                LastMessage = c.Messages.OrderByDescending(m => m.SentAt).Select(m => m.Content).FirstOrDefault() ?? "",
                LastMessageAt = c.Messages.OrderByDescending(m => m.SentAt).Select(m => (DateTime?)m.SentAt).FirstOrDefault()
            })
            .ToListAsync();

        return Ok(conversations);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ConversationDto>> GetById(Guid id)
    {
        var c = await _context.Conversations
            .Include(c => c.Contact)
            .Include(c => c.AssignedUser)
            .Include(c => c.Messages)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (c == null) return NotFound();

        return Ok(new ConversationDto
        {
            Id = c.Id,
            ContactId = c.ContactId,
            ContactName = c.Contact.Name,
            ContactPhone = c.Contact.PhoneNumber,
            AssignedUserId = c.AssignedUserId,
            AssignedUserName = c.AssignedUser?.Name,
            Status = c.Status.ToString(),
            CreatedAt = c.CreatedAt,
            UpdatedAt = c.UpdatedAt,
            LastMessage = c.Messages.OrderByDescending(m => m.SentAt).Select(m => m.Content).FirstOrDefault() ?? "",
            LastMessageAt = c.Messages.OrderByDescending(m => m.SentAt).Select(m => (DateTime?)m.SentAt).FirstOrDefault()
        });
    }

    [HttpGet("{id}/messages")]
    public async Task<ActionResult<IEnumerable<MessageDto>>> GetMessages(Guid id)
    {
        var messages = await _context.Messages
            .Where(m => m.ConversationId == id)
            .OrderBy(m => m.SentAt)
            .Select(m => new MessageDto
            {
                Id = m.Id,
                ConversationId = m.ConversationId,
                Content = m.Content,
                IsFromContact = m.IsFromContact,
                SentAt = m.SentAt
            })
            .ToListAsync();

        return Ok(messages);
    }

    [HttpPost("{id}/messages")]
    public async Task<ActionResult<MessageDto>> SendMessage(Guid id, [FromBody] SendMessageRequest request)
    {
        var conversation = await _context.Conversations
            .Include(c => c.Contact)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (conversation == null) return NotFound();

        var message = new Message
        {
            ConversationId = id,
            Content = request.Content,
            IsFromContact = false,
            SentAt = DateTime.UtcNow,
            CompanyId = _tenantService.CompanyId
        };

        _context.Messages.Add(message);
        conversation.Status = ConversationStatus.InProgress;
        conversation.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        var dto = new MessageDto
        {
            Id = message.Id,
            ConversationId = message.ConversationId,
            Content = message.Content,
            IsFromContact = message.IsFromContact,
            SentAt = message.SentAt
        };

        // Notify via SignalR
        await _hubContext.Clients.Group(_tenantService.CompanyId.ToString())
            .SendAsync("ReceiveMessage", dto);

        return Ok(dto);
    }

    [HttpPut("{id}/assign")]
    public async Task<IActionResult> Assign(Guid id, [FromBody] AssignConversationRequest request)
    {
        var conversation = await _context.Conversations.FirstOrDefaultAsync(c => c.Id == id);
        if (conversation == null) return NotFound();

        conversation.AssignedUserId = request.UserId;
        conversation.Status = ConversationStatus.InProgress;
        conversation.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpPut("{id}/status")]
    public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] UpdateConversationStatusRequest request)
    {
        var conversation = await _context.Conversations.FirstOrDefaultAsync(c => c.Id == id);
        if (conversation == null) return NotFound();

        if (!Enum.TryParse<ConversationStatus>(request.Status, out var status))
            return BadRequest("Invalid status.");

        conversation.Status = status;
        conversation.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
