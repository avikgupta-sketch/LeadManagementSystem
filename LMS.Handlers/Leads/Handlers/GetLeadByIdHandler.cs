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

        // 🔴 NULL SAFETY
        if (lead == null)
            return null;

        return new LeadDetailDto
        {
            Id = lead.Id,
            Title = lead.Title,
            Description = lead.Description,
            Status = lead.Status.ToString(),

            Remarks = lead.Remarks
                .OrderByDescending(r => r.CreatedDate)
                .Select(r => new LeadRemarkDto
                {
                    Id = r.Id,
                    Remark = r.Remark,
                    OldStatus = r.OldStatus.ToString(),
                    NewStatus = r.NewStatus.ToString(),
                    CreatedDate = r.CreatedDate,
                    // 🔴 NULL SAFETY: ChangedBy may have been hidden by query filter
                    ChangedByName = r.ChangedBy?.FullName ?? "Unknown"
                }).ToList()
        };
    }
}
