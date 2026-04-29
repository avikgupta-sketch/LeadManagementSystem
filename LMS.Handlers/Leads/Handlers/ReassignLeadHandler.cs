using LMS.Data.Context;
using MediatR;
using LMS.Handlers.Leads.Commands;
using LMS.Models.Entities;

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
        var dto = request.Dto;

        var lead = _context.Leads
            .FirstOrDefault(l => l.Id == dto.LeadId);

        if (lead == null)
            return false;

        // 🔴 Check manager ownership
        if (lead.ManagerId != request.ManagerId)
            return false;

        // 🔴 Validate new agent belongs to manager
        var agent = _context.Users
            .FirstOrDefault(u => u.Id == dto.NewAgentId && u.ManagerId == request.ManagerId);

        if (agent == null)
            return false;

        var oldAgentId = lead.AssignedAgentId;

        // 🔥 Reassign
        lead.AssignedAgentId = dto.NewAgentId;
        lead.UpdatedDate = DateTime.UtcNow;

        // 🔥 Add remark (audit)
        var remark = new LeadRemark
        {
            LeadId = lead.Id,
            ChangedById = request.ManagerId,
            Remark = $"Lead reassigned from Agent {oldAgentId} to {dto.NewAgentId}",
            OldStatus = lead.Status,
            NewStatus = lead.Status,
            CreatedById = request.ManagerId
        };

        _context.LeadRemarks.Add(remark);

        await _context.SaveChangesAsync();

        return true;
    }
}
