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
public class AppointmentsController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public AppointmentsController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<AppointmentDto>>> GetAll()
    {
        var appointments = await _context.Appointments
            .Include(a => a.Contact)
            .Select(a => new AppointmentDto
            {
                Id = a.Id,
                ContactId = a.ContactId,
                ContactName = a.Contact.Name,
                AppointmentDate = a.AppointmentDate,
                Type = a.Type.ToString(),
                Notes = a.Notes,
                CompanyId = a.CompanyId
            })
            .ToListAsync();

        return Ok(appointments);
    }

    [HttpPost]
    public async Task<ActionResult<AppointmentDto>> Create([FromBody] CreateAppointmentRequest request)
    {
        var contact = await _context.Contacts.FirstOrDefaultAsync(c => c.Id == request.ContactId);
        if (contact == null) return NotFound("Contact not found.");

        if (!Enum.TryParse<AppointmentType>(request.Type, out var type))
            type = AppointmentType.Support;

        var appointment = new Appointment
        {
            ContactId = request.ContactId,
            AppointmentDate = request.AppointmentDate,
            Type = type,
            Notes = request.Notes
        };

        _context.Appointments.Add(appointment);
        await _context.SaveChangesAsync();

        return Ok(new AppointmentDto
        {
            Id = appointment.Id,
            ContactId = appointment.ContactId,
            ContactName = contact.Name,
            AppointmentDate = appointment.AppointmentDate,
            Type = appointment.Type.ToString(),
            Notes = appointment.Notes,
            CompanyId = appointment.CompanyId
        });
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateAppointmentRequest request)
    {
        var appointment = await _context.Appointments.FirstOrDefaultAsync(a => a.Id == id);
        if (appointment == null) return NotFound();

        if (!Enum.TryParse<AppointmentType>(request.Type, out var type))
            type = AppointmentType.Support;

        appointment.AppointmentDate = request.AppointmentDate;
        appointment.Type = type;
        appointment.Notes = request.Notes;
        appointment.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var appointment = await _context.Appointments.FirstOrDefaultAsync(a => a.Id == id);
        if (appointment == null) return NotFound();

        _context.Appointments.Remove(appointment);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
