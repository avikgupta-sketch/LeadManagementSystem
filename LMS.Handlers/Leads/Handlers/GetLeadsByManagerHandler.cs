using LMS.Data.Context;
using LMS.Handlers.Leads.Queries;
using LMS.Models.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LMS.Handlers.Leads.Handlers;

public class GetLeadsByManagerHandler : IRequestHandler<GetLeadsByManagerQuery, List<Lead>>
{
    private readonly AppDbContext _context;

    public GetLeadsByManagerHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Lead>> Handle(GetLeadsByManagerQuery request, CancellationToken cancellationToken)
    {
        // Fetch leads (global filter excludes IsDeleted leads)
        var leads = await _context.Leads
            .AsNoTracking()
            .Where(l => l.ManagerId == request.ManagerId)
            .OrderByDescending(l => l.CreatedDate)
            .ToListAsync(cancellationToken);

        if (leads.Count == 0) return leads;

        
        var agentIds = leads.Select(l => l.AssignedAgentId).Distinct().ToList();

        var agents = await _context.Users
            .AsNoTracking()
            .IgnoreQueryFilters()
            .Where(u => agentIds.Contains(u.Id))
            .ToDictionaryAsync(u => u.Id, cancellationToken);

        foreach (var lead in leads)
        {
            if (agents.TryGetValue(lead.AssignedAgentId, out var agent))
                lead.AssignedAgent = agent;
        }

        return leads;
    }
}
