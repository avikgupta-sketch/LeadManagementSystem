using LMS.Data.Context;
using LMS.Models.Entities;
using MediatR;
using LMS.Handlers.Leads.Commands;

namespace LMS.Handlers.Leads.Handlers;

public class UpdateLeadStatusHandler
    : IRequestHandler<UpdateLeadStatusCommand, bool>
{
    private readonly AppDbContext _context;

    public UpdateLeadStatusHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(UpdateLeadStatusCommand request, CancellationToken cancellationToken)
    {
        var dto = request.Dto;

        var lead = _context.Leads
            .FirstOrDefault(l => l.Id == dto.LeadId && l.AssignedAgentId == request.AgentId);

        if (lead == null)
            return false;

        var oldStatus = lead.Status;

        // 🔥 Update status
        lead.Status = dto.NewStatus;
        lead.UpdatedById = request.AgentId;
        lead.UpdatedDate = DateTime.UtcNow;

        // 🔥 Create remark (AUDIT)
        var remark = new LeadRemark
        {
            LeadId = lead.Id,
            ChangedById = request.AgentId,
            Remark = dto.Remark,
            OldStatus = oldStatus,
            NewStatus = dto.NewStatus,
            CreatedById = request.AgentId
        };

        _context.LeadRemarks.Add(remark);

        await _context.SaveChangesAsync();

        return true;
    }
}