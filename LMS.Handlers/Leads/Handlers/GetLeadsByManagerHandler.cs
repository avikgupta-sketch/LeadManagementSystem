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
        // global query filter excludes IsDeleted leads
        return await _context.Leads
            .AsNoTracking()
            .Include(l => l.AssignedAgent)
            .Where(l => l.ManagerId == request.ManagerId)
            .OrderByDescending(l => l.CreatedDate)
            .ToListAsync(cancellationToken);
    }
}
