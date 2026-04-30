using LMS.Data.Context;
using LMS.Handlers.Leads.Queries;
using LMS.Models.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LMS.Handlers.Leads.Handlers;

public class GetLeadsByAgentHandler
    : IRequestHandler<GetLeadsByAgentQuery, List<Lead>>
{
    private readonly AppDbContext _context;

    public GetLeadsByAgentHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Lead>> Handle(
        GetLeadsByAgentQuery request,
        CancellationToken cancellationToken)
    {
        // 1) Fetch leads (global filter excludes IsDeleted leads)
        var leads = await _context.Leads
            .AsNoTracking()
            .Where(l => l.AssignedAgentId == request.AgentId)
            .OrderByDescending(l => l.CreatedDate)
            .ToListAsync(cancellationToken);

        if (leads.Count == 0) return leads;

        // 2) Fetch the agent record (even if soft-deleted) so the view
        //    never breaks on a null navigation property.
        var agent = await _context.Users
            .AsNoTracking()
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(u => u.Id == request.AgentId, cancellationToken);

        if (agent != null)
        {
            foreach (var lead in leads)
                lead.AssignedAgent = agent;
        }

        return leads;
    }
}
