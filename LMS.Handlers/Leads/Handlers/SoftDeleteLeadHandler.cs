using LMS.Data.Context;
using LMS.Handlers.Leads.Commands;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Serilog;

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
        try
        {
            var lead = await _context.Leads
                .FirstOrDefaultAsync(l => l.Id == request.LeadId, cancellationToken);

            if (lead == null)
                return false;

            
            if (lead.ManagerId != request.ManagerId)
                return false;

            lead.IsDeleted = true;
            lead.UpdatedById = request.ManagerId;
            lead.UpdatedDate = DateTime.UtcNow;

            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "SoftDeleteLeadHandler failed for LeadId={LeadId}", request.LeadId);
            return false;
        }
    }
}
