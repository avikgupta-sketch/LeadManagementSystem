using LMS.Data.Context;
using LMS.Handlers.Leads.Commands;
using LMS.Models.Entities;
using LMS.Models.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Serilog;

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
        try
        {
            var dto = request.Dto;

            
            if (string.IsNullOrWhiteSpace(dto.Remark))
                return false;

            var lead = await _context.Leads
                .FirstOrDefaultAsync(
                    l => l.Id == dto.LeadId && l.AssignedAgentId == request.AgentId,
                    cancellationToken);

            
            if (lead == null)
                return false;

            
            if (lead.Status == LeadStatus.Converted ||
                lead.Status == LeadStatus.Closed ||
                lead.Status == LeadStatus.Rejected)
            {
                return false;
            }

            var oldStatus = lead.Status;

            
            if (oldStatus == dto.NewStatus)
                return false;

            lead.Status = dto.NewStatus;
            lead.UpdatedById = request.AgentId;
            lead.UpdatedDate = DateTime.UtcNow;

            //  Mandatory audit remark
            var remark = new LeadRemark
            {
                LeadId = lead.Id,
                ChangedById = request.AgentId,
                Remark = dto.Remark.Trim(),
                OldStatus = oldStatus,
                NewStatus = dto.NewStatus,
                CreatedById = request.AgentId
            };

            _context.LeadRemarks.Add(remark);

            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "UpdateLeadStatusHandler failed for LeadId={LeadId}", request.Dto.LeadId);
            return false;
        }
    }
}
