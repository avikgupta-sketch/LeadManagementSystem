using LMS.Data.Context;
using LMS.Models.Entities;
using MediatR;
using LMS.Handlers.Leads.Queries;

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
        return _context.Leads
            .Where(l => l.AssignedAgentId == request.AgentId)
            .ToList();
    }
}
