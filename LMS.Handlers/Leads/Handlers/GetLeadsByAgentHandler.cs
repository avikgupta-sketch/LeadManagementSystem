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
        // global query filter excludes IsDeleted leads
        return await _context.Leads
            .AsNoTracking()
            .Include(l => l.AssignedAgent)
            .Where(l => l.AssignedAgentId == request.AgentId)
            .OrderByDescending(l => l.CreatedDate)
            .ToListAsync(cancellationToken);
    }
}
