using LMS.Data.Context;
using MediatR;
using LMS.Handlers.Leads.Commands;

namespace LMS.Handlers.Leads.Handlers;

public class SoftDeleteLeadHandler : IRequestHandler<SoftDeleteLeadCommand, bool>
{
    private readonly AppDbContext _context;

    public SoftDeleteLeadHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(SoftDeleteLeadCommand request, CancellationToken cancellationToken)
    {
        var lead = _context.Leads
            .FirstOrDefault(l => l.Id == request.LeadId);

        if (lead == null)
            return false;

        // 🔴 Ensure manager owns the lead
        if (lead.ManagerId != request.ManagerId)
            return false;

        lead.IsDeleted = true;
        lead.UpdatedDate = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return true;
    }
}
