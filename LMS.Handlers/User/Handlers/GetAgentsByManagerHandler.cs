using LMS.Data.Context;
using LMS.Handlers.Users.Queries;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LMS.Handlers.Users.Handlers;

public class GetAgentsByManagerHandler
    : IRequestHandler<GetAgentsByManagerQuery, List<(int, string)>>
{
    private readonly AppDbContext _context;

    public GetAgentsByManagerHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<(int, string)>> Handle(
        GetAgentsByManagerQuery request,
        CancellationToken cancellationToken)
    {
        // global query filter excludes IsDeleted
        var rows = await _context.Users
            .Where(u => u.ManagerId == request.ManagerId)
            .OrderBy(u => u.FullName)
            .Select(u => new { u.Id, u.FullName })
            .ToListAsync(cancellationToken);

        return rows
            .Select(u => (u.Id, u.FullName ?? string.Empty))
            .ToList();
    }
}
