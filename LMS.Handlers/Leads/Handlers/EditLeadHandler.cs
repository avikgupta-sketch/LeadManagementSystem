using LMS.Data.Context;
using LMS.Handlers.Leads.Commands;
using LMS.Models.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace LMS.Handlers.Leads.Handlers;

public class EditLeadHandler : IRequestHandler<EditLeadCommand, bool>
{
    private readonly AppDbContext _context;

    public EditLeadHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(EditLeadCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var dto = request.Dto;

            // Null safety + validation
            if (string.IsNullOrWhiteSpace(dto.Name) ||
                string.IsNullOrWhiteSpace(dto.Description) ||
                string.IsNullOrWhiteSpace(dto.PhoneNumber) ||
                string.IsNullOrWhiteSpace(dto.Email) ||
                string.IsNullOrWhiteSpace(dto.Address))
            {
                return false;
            }

            var lead = await _context.Leads
                .FirstOrDefaultAsync(l => l.Id == dto.Id, cancellationToken);

            if (lead == null)
                return false;

            // 🔴 Cannot edit terminal status leads
            if (lead.Status == LeadStatus.Converted ||
                lead.Status == LeadStatus.Closed ||
                lead.Status == LeadStatus.Rejected)
            {
                return false;
            }

            // 🔴 Authorization
            if (request.IsManager)
            {
                if (lead.ManagerId != request.RequestedById)
                    return false;
            }
            else
            {
                if (lead.AssignedAgentId != request.RequestedById)
                    return false;
            }

            lead.Name = dto.Name.Trim();
            lead.Description = dto.Description.Trim();
            lead.PhoneNumber = dto.PhoneNumber.Trim();
            lead.Email = dto.Email.Trim();
            lead.Address = dto.Address.Trim();
            lead.Gender = dto.Gender;
            lead.UpdatedById = request.RequestedById;
            lead.UpdatedDate = DateTime.UtcNow;

            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "EditLeadHandler failed for LeadId={LeadId}", request.Dto.Id);
            return false;
        }
    }
}

