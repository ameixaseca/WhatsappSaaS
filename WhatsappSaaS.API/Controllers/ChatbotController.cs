using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WhatsappSaaS.Application.DTOs;
using WhatsappSaaS.Domain.Entities;
using WhatsappSaaS.Infrastructure.Persistence;

namespace WhatsappSaaS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ChatbotController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public ChatbotController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ChatbotFlowDto>>> GetAll()
    {
        var flows = await _context.ChatbotFlows
            .Include(f => f.Steps)
            .Select(f => new ChatbotFlowDto
            {
                Id = f.Id,
                Name = f.Name,
                IsActive = f.IsActive,
                CompanyId = f.CompanyId,
                Steps = f.Steps.OrderBy(s => s.Order).Select(s => new FlowStepDto
                {
                    Id = s.Id,
                    Content = s.Content,
                    Type = s.Type.ToString(),
                    Order = s.Order,
                    OptionsJson = s.OptionsJson
                }).ToList()
            })
            .ToListAsync();

        return Ok(flows);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ChatbotFlowDto>> GetById(Guid id)
    {
        var f = await _context.ChatbotFlows
            .Include(f => f.Steps)
            .FirstOrDefaultAsync(f => f.Id == id);

        if (f == null) return NotFound();

        return Ok(new ChatbotFlowDto
        {
            Id = f.Id,
            Name = f.Name,
            IsActive = f.IsActive,
            CompanyId = f.CompanyId,
            Steps = f.Steps.OrderBy(s => s.Order).Select(s => new FlowStepDto
            {
                Id = s.Id,
                Content = s.Content,
                Type = s.Type.ToString(),
                Order = s.Order,
                OptionsJson = s.OptionsJson
            }).ToList()
        });
    }

    [HttpPost]
    public async Task<ActionResult<ChatbotFlowDto>> Create([FromBody] CreateChatbotFlowRequest request)
    {
        var flow = new ChatbotFlow
        {
            Name = request.Name,
            IsActive = request.IsActive
        };

        _context.ChatbotFlows.Add(flow);
        await _context.SaveChangesAsync();

        foreach (var step in request.Steps)
        {
            if (!Enum.TryParse<FlowStepType>(step.Type, out var stepType))
                stepType = FlowStepType.Response;

            var flowStep = new FlowStep
            {
                ChatbotFlowId = flow.Id,
                Content = step.Content,
                Type = stepType,
                Order = step.Order,
                OptionsJson = step.OptionsJson,
                CompanyId = flow.CompanyId
            };
            _context.FlowSteps.Add(flowStep);
        }

        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = flow.Id }, new ChatbotFlowDto
        {
            Id = flow.Id,
            Name = flow.Name,
            IsActive = flow.IsActive,
            CompanyId = flow.CompanyId
        });
    }

    [HttpPut("{id}/toggle")]
    public async Task<IActionResult> Toggle(Guid id)
    {
        var flow = await _context.ChatbotFlows.FirstOrDefaultAsync(f => f.Id == id);
        if (flow == null) return NotFound();

        flow.IsActive = !flow.IsActive;
        flow.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var flow = await _context.ChatbotFlows.Include(f => f.Steps).FirstOrDefaultAsync(f => f.Id == id);
        if (flow == null) return NotFound();

        _context.FlowSteps.RemoveRange(flow.Steps);
        _context.ChatbotFlows.Remove(flow);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
