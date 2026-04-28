using LMS.Data.Context;
using MediatR;
using LMS.Handlers.Users.Queries;

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
        return _context.Users
    .Where(u => u.ManagerId == request.ManagerId)
    .Select(u => new { u.Id, u.FullName })
    .ToList()
    .Select(u => (u.Id, u.FullName))
    .ToList();
    }
}
