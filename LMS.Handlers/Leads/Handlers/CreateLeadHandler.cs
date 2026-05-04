using LMS.Data.Context;
using LMS.Handlers.Leads.Commands;
using LMS.Models.Entities;
using LMS.Models.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace LMS.Handlers.Leads.Handlers;

public class CreateLeadHandler : IRequestHandler<CreateLeadCommand, bool>
{
    private readonly AppDbContext _context;

    public CreateLeadHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(CreateLeadCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var dto = request.Dto;

            // DATA VALIDATION
            if (string.IsNullOrWhiteSpace(dto.Name) ||
                string.IsNullOrWhiteSpace(dto.Description) ||
                string.IsNullOrWhiteSpace(dto.PhoneNumber) ||
                string.IsNullOrWhiteSpace(dto.Email) ||
                string.IsNullOrWhiteSpace(dto.Address))
            {
                return false;
            }

            
            var agent = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == dto.AssignedAgentId, cancellationToken);

            if (agent == null)
                return false;

            
            if (agent.ManagerId != request.ManagerId)
                return false;

            var lead = new Lead
            {
                Name = dto.Name.Trim(),
                Description = dto.Description.Trim(),
                PhoneNumber = dto.PhoneNumber.Trim(),
                Email = dto.Email.Trim(),
                Address = dto.Address.Trim(),
                Gender = dto.Gender,
                AssignedAgentId = dto.AssignedAgentId,
                ManagerId = request.ManagerId,
                CreatedById = request.CreatedById,
                Status = LeadStatus.New
            };

            _context.Leads.Add(lead);
            await _context.SaveChangesAsync(cancellationToken);

            // Initial audit remark
            var remark = new LeadRemark
            {
                LeadId = lead.Id,
                ChangedById = request.CreatedById,
                Remark = "Lead created",
                OldStatus = LeadStatus.New,
                NewStatus = LeadStatus.New,
                CreatedById = request.CreatedById
            };

            _context.LeadRemarks.Add(remark);
            await _context.SaveChangesAsync(cancellationToken);

            return true;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "CreateLeadHandler failed (CreatedById={CreatedById}, ManagerId={ManagerId})",
                request.CreatedById, request.ManagerId);
            return false;
        }
    }
}