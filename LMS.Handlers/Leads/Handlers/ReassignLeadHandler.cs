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

            // 🔴 AUTHORIZATION: this manager must own the lead
            if (lead.ManagerId != request.ManagerId)
                return false;

            // 🔴 EDIT RESTRICTION: cannot reassign a terminal-status lead
            if (lead.Status == LeadStatus.Converted ||
                lead.Status == LeadStatus.Closed ||
                lead.Status == LeadStatus.Rejected)
            {
                return false;
            }

            // 🔴 CONSISTENCY: new agent must belong to the same manager
            var agent = await _context.Users
                .FirstOrDefaultAsync(
                    u => u.Id == dto.NewAgentId && u.ManagerId == request.ManagerId,
                    cancellationToken);

            if (agent == null)
                return false;

            var oldAgentId = lead.AssignedAgentId;
            if (oldAgentId == dto.NewAgentId)
                return false;

            lead.AssignedAgentId = dto.NewAgentId;
            lead.UpdatedById = request.ManagerId;
            lead.UpdatedDate = DateTime.UtcNow;

            // 🔥 Audit remark (status itself does not change)
            var remark = new LeadRemark
            {
                LeadId = lead.Id,
                ChangedById = request.ManagerId,
                Remark = $"Lead reassigned from Agent {oldAgentId} to Agent {dto.NewAgentId}",
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
