using LMS.Data.Context;
using LMS.Handlers.Leads.Commands;
using LMS.Models.Entities;
using LMS.Models.Enums;
using MediatR;

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
        var dto = request.Dto;

        var agent = _context.Users
            .FirstOrDefault(u => u.Id == dto.AssignedAgentId);

        if (agent == null)
            return false;

        // 🔥 If Manager → validate ownership
        if (agent.ManagerId != request.ManagerId)
            return false;

        var lead = new Lead
        {
            Title = dto.Title,
            Description = dto.Description,
            AssignedAgentId = dto.AssignedAgentId,
            ManagerId = request.ManagerId,
            CreatedById = request.CreatedById,
            Status = LeadStatus.New
        };

        _context.Leads.Add(lead);
        await _context.SaveChangesAsync();

        // 🔥 OPTIONAL BUT RECOMMENDED (initial remark)
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
        await _context.SaveChangesAsync();

        return true;
    }
}