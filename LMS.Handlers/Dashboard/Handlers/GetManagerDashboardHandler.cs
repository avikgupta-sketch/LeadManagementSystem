using LMS.Data.Context;
using LMS.Models.DTOs.Dashboard;
using LMS.Models.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;


namespace LMS.Handlers.Dashboard.Handlers;

public class GetManagerDashboardHandler
    : IRequestHandler<GetManagerDashboardQuery, DashboardDto>
{
    private readonly AppDbContext _context;

    public GetManagerDashboardHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<DashboardDto> Handle(
        GetManagerDashboardQuery request,
        CancellationToken cancellationToken)
    {
        var leads = await _context.Leads
            .Where(l => l.ManagerId == request.ManagerId)
            .ToListAsync(cancellationToken);

        return new DashboardDto
        {
            TotalLeads = leads.Count,

            New = leads.Count(l => l.Status == LeadStatus.New),
            InProgress = leads.Count(l => l.Status == LeadStatus.InProgress),
            FollowUp = leads.Count(l => l.Status == LeadStatus.FollowUp),
            Interested = leads.Count(l => l.Status == LeadStatus.Interested),
            NotInterested = leads.Count(l => l.Status == LeadStatus.NotInterested),
            Converted = leads.Count(l => l.Status == LeadStatus.Converted),
            Closed = leads.Count(l => l.Status == LeadStatus.Closed),
            Rejected = leads.Count(l => l.Status == LeadStatus.Rejected)
        };
    }
}