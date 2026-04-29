using LMS.Data.Context;
using LMS.Handlers.Leads.Queries;
using LMS.Models.Entities;
using MediatR;

public class GetLeadsByManagerHandler : IRequestHandler<GetLeadsByManagerQuery, List<Lead>>
{
    private readonly AppDbContext _context;

    public GetLeadsByManagerHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Lead>> Handle(GetLeadsByManagerQuery request, CancellationToken cancellationToken)
    {
        return _context.Leads
            .Where(l => l.ManagerId == request.ManagerId)
            .ToList();
    }
}
