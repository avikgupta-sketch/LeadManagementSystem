using LMS.Data.Context;
using LMS.Handlers.Leads.Commands;
using LMS.Models.Entities;
using LMS.Models.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace LMS.Handlers.Leads.Handlers;

public class ReassignLeadHandler : IRequestHandler<ReassignLeadCommand, bool>
{
    private readonly AppDbContext _context;

    public ReassignLeadHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(ReassignLeadCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var dto = request.Dto;

            var lead = await _context.Leads
                .FirstOrDefaultAsync(l => l.Id == dto.LeadId, cancellationToken);

            if (lead == null)
                return false;

            
            if (lead.ManagerId != request.ManagerId)
                return false;

            
            if (lead.Status == LeadStatus.Converted ||
                lead.Status == LeadStatus.Closed ||
                lead.Status == LeadStatus.Rejected)
            {
                return false;
            }

            
            var agent = await _context.Users
                .FirstOrDefaultAsync(
                    u => u.Id == dto.NewAgentId && u.ManagerId == request.ManagerId,
                    cancellationToken);

            if (agent == null)
                return false;

            var oldAgentId = lead.AssignedAgentId;
            
            if (oldAgentId == dto.NewAgentId)
                return false;
            var oldAgent = await _context.Users
    .FirstOrDefaultAsync(u => u.Id == oldAgentId, cancellationToken);

            string oldAgentName = oldAgent?.FullName ?? "Unknown Agent";  
            string newAgentName = agent.FullName;  

            lead.AssignedAgentId = dto.NewAgentId;
            lead.UpdatedById = request.ManagerId;
            lead.UpdatedDate = DateTime.UtcNow;

            //  Audit remark 
            var remark = new LeadRemark
            {
                LeadId = lead.Id,
                ChangedById = request.ManagerId,
                Remark = $"Lead reassigned from {oldAgentName} to {newAgentName}",
                OldStatus = lead.Status,
                NewStatus = lead.Status,
                CreatedById = request.ManagerId
            };

            _context.LeadRemarks.Add(remark);
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "ReassignLeadHandler failed for LeadId={LeadId}", request.Dto.LeadId);
            return false;
        }
    }
}
