using LMS.Data.Context;
using LMS.Handlers.Leads.Queries;
using LMS.Models.DTOs.Lead;
using LMS.Models.DTOs.LeadRemark;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LMS.Handlers.Leads.Handlers;

public class GetLeadByIdHandler : IRequestHandler<GetLeadByIdQuery, LeadDetailDto?>
{
    private readonly AppDbContext _context;

    public GetLeadByIdHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<LeadDetailDto?> Handle(GetLeadByIdQuery request, CancellationToken cancellationToken)
    {
        var lead = await _context.Leads
            .AsNoTracking()
            .Include(l => l.Remarks)
                .ThenInclude(r => r.ChangedBy)
            .FirstOrDefaultAsync(l => l.Id == request.LeadId, cancellationToken);

        if (lead == null)
            return null;

        
        var agent = await _context.Users
            .AsNoTracking()
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(u => u.Id == lead.AssignedAgentId, cancellationToken);

        return new LeadDetailDto
        {
            Id = lead.Id,
            Name = lead.Name,
            Description = lead.Description,
            PhoneNumber = lead.PhoneNumber,
            Email = lead.Email,
            Address = lead.Address,
            Gender = lead.Gender,
            Status = lead.Status,

            AssignedAgentId = lead.AssignedAgentId,
            AssignedAgentName = agent?.FullName ?? $"Agent #{lead.AssignedAgentId}",
            AssignedAgentDeleted = agent?.IsDeleted ?? true,

            Remarks = lead.Remarks
                .OrderByDescending(r => r.CreatedDate)
                .Select(r => new LeadRemarkDto
                {
                    Id = r.Id,
                    Remark = r.Remark,
                    OldStatus = r.OldStatus.ToString(),
                    NewStatus = r.NewStatus.ToString(),
                    CreatedDate = r.CreatedDate,
                    ChangedByName = r.ChangedBy?.FullName ?? "Unknown"
                }).ToList()
        };
    }
}
