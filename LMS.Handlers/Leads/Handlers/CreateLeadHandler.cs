using LMS.Data.Context;
using LMS.Handlers.Leads.Commands;
using LMS.Models.Entities;
using LMS.Models.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace LMS.Handlers.Leads.Handlers;

public class CreateLeadHandler : IRequestHandler<CreateLeadCommand, bool>
{
    private readonly AppDbContext _context;

    public CreateLeadHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(CreateLeadCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var dto = request.Dto;

            // 🔴 DATA VALIDATION: title / description must not be null/empty
            if (string.IsNullOrWhiteSpace(dto.Title) || string.IsNullOrWhiteSpace(dto.Description))
                return false;

            // 🔴 DATA VALIDATION: invalid AgentId → reject
            var agent = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == dto.AssignedAgentId, cancellationToken);

            if (agent == null)
                return false;

            // 🔴 AUTHORIZATION: agent must belong to the manager that owns this lead
            //    (also blocks Manager from assigning leads to themselves
            //     because a Manager's ManagerId is null, not their own Id).
            if (agent.ManagerId != request.ManagerId)
                return false;

            var lead = new Lead
            {
                Title = dto.Title.Trim(),
                Description = dto.Description.Trim(),
                AssignedAgentId = dto.AssignedAgentId,
                ManagerId = request.ManagerId,
                CreatedById = request.CreatedById,
                Status = LeadStatus.New
            };

            _context.Leads.Add(lead);
            await _context.SaveChangesAsync(cancellationToken);

            // Initial audit remark
            var remark = new LeadRemark
            {
                LeadId = lead.Id,
                ChangedById = request.CreatedById,
                Remark = "Lead created",
                OldStatus = LeadStatus.New,
                NewStatus = LeadStatus.New,
                CreatedById = request.CreatedById
            };

            _context.LeadRemarks.Add(remark);
            await _context.SaveChangesAsync(cancellationToken);

            return true;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "CreateLeadHandler failed (CreatedById={CreatedById}, ManagerId={ManagerId})",
                request.CreatedById, request.ManagerId);
            return false;
        }
    }
}
