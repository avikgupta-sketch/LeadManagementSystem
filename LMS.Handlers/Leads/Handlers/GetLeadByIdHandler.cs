using LMS.Data.Context;
using MediatR;
using Microsoft.EntityFrameworkCore;
using LMS.Handlers.Leads.Queries;
using LMS.Models.DTOs.Lead;
using LMS.Models.DTOs.LeadRemark;

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
            .Include(l => l.Remarks)
            .ThenInclude(r => r.ChangedBy)
            .FirstOrDefaultAsync(l => l.Id == request.LeadId);

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
                    ChangedByName = r.ChangedBy.FullName
                }).ToList()
        };
    }
}