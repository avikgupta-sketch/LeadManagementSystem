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
            if (string.IsNullOrWhiteSpace(dto.Title) || string.IsNullOrWhiteSpace(dto.Description))
                return false;

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
                // Manager may edit only leads belonging to their agents
                if (lead.ManagerId != request.RequestedById)
                    return false;
            }
            else
            {
                // Agent may edit only their own leads
                if (lead.AssignedAgentId != request.RequestedById)
                    return false;
            }

            lead.Title = dto.Title.Trim();
            lead.Description = dto.Description.Trim();
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
