using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using WhatsappSaaS.Application.DTOs;
using WhatsappSaaS.Domain.Entities;
using WhatsappSaaS.Infrastructure.Hubs;
using WhatsappSaaS.Infrastructure.Persistence;

namespace WhatsappSaaS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WebhookController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IHubContext<ChatHub> _hubContext;

    public WebhookController(ApplicationDbContext context, IHubContext<ChatHub> hubContext)
    {
        _context = context;
        _hubContext = hubContext;
    }

    /// <summary>
    /// Receives incoming WhatsApp messages via Twilio webhook.
    /// </summary>
    [HttpPost("twilio")]
    public async Task<IActionResult> TwilioWebhook([FromForm] WhatsAppWebhookPayload payload)
    {
        await ProcessIncomingMessage(payload.From, payload.Body);
        // Twilio expects a TwiML response (empty is fine for no auto-reply)
        return Content("<Response></Response>", "text/xml");
    }

    /// <summary>
    /// Receives incoming WhatsApp messages via 360dialog webhook.
    /// </summary>
    [HttpPost("360dialog")]
    public async Task<IActionResult> Dialog360Webhook([FromBody] Dialog360WebhookPayload payload)
    {
        if (payload?.Messages != null)
        {
            foreach (var msg in payload.Messages)
            {
                if (msg.Type == "text" && msg.Text != null)
                {
                    await ProcessIncomingMessage(msg.From, msg.Text.Body);
                }
            }
        }
        return Ok();
    }

    private async Task ProcessIncomingMessage(string from, string messageBody)
    {
        // Find which company owns this number (for MVP, use the first active company)
        var company = await _context.Companies
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(c => c.IsActive);

        if (company == null) return;

        // Find or create contact
        var contact = await _context.Contacts
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(c => c.PhoneNumber == from && c.CompanyId == company.Id);

        if (contact == null)
        {
            contact = new Contact
            {
                Name = from,
                PhoneNumber = from,
                CompanyId = company.Id
            };
            _context.Contacts.Add(contact);
            await _context.SaveChangesAsync();
        }

        // Find or create conversation
        var conversation = await _context.Conversations
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(c => c.ContactId == contact.Id && c.Status != ConversationStatus.Finished);

        if (conversation == null)
        {
            conversation = new Conversation
            {
                ContactId = contact.Id,
                Status = ConversationStatus.New,
                CompanyId = company.Id
            };
            _context.Conversations.Add(conversation);
            await _context.SaveChangesAsync();
        }

        // Create message
        var message = new Message
        {
            ConversationId = conversation.Id,
            Content = messageBody,
            IsFromContact = true,
            SentAt = DateTime.UtcNow,
            CompanyId = company.Id
        };

        _context.Messages.Add(message);
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

        // Notify connected clients via SignalR
        await _hubContext.Clients.Group(company.Id.ToString()).SendAsync("ReceiveMessage", dto);
    }
}

public class Dialog360WebhookPayload
{
    public Dialog360Message[]? Messages { get; set; }
}

public class Dialog360Message
{
    public string From { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public Dialog360Text? Text { get; set; }
}

public class Dialog360Text
{
    public string Body { get; set; } = string.Empty;
}
