using LMS.Data.Context;
using LMS.Handlers.Leads.Queries;
using LMS.Models.DTOs.Lead;
using LMS.Models.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace LMS.Handlers.Leads.Handlers;

public class GetLeadsDataTableHandler
    : IRequestHandler<GetLeadsDataTableQuery, DataTableResponseDto<LeadTableRowDto>>
{
    private readonly AppDbContext _context;

    public GetLeadsDataTableHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<DataTableResponseDto<LeadTableRowDto>> Handle(
        GetLeadsDataTableQuery request,
        CancellationToken cancellationToken)
    {
        var req = request.Request;

        // Base query — filter by role
        var query = _context.Leads
            .AsNoTracking()
            .IgnoreQueryFilters()                      // we apply soft-delete manually below
            .Where(l => !l.IsDeleted)
            .Where(l => request.IsManager
                ? l.ManagerId == request.UserId
                : l.AssignedAgentId == request.UserId);

        // Total before search
        int total = await query.CountAsync(cancellationToken);

        // Search — filter Name, Email, PhoneNumber, Status
        if (!string.IsNullOrWhiteSpace(req.SearchValue))
        {
            var s = req.SearchValue.ToLower();

            // Status enum is resolved in memory first, then passed to SQL
            // because EF Core cannot translate .ToString() on an enum to SQL Server
            var matchingStatuses = Enum.GetValues<LeadStatus>()
                .Where(e => e.ToString().ToLower().Contains(s))
                .ToList();

            query = query.Where(l =>
                l.Name.ToLower().Contains(s) ||
                l.Email.ToLower().Contains(s) ||
                l.PhoneNumber.ToLower().Contains(s) ||
                matchingStatuses.Contains(l.Status));  // ✅ EF Core CAN translate this
        }

        // Total after search
        int filtered = await query.CountAsync(cancellationToken);

        // Sort
        query = (req.OrderColumn?.ToLower(), req.OrderDir?.ToLower()) switch
        {
            ("status", "asc") => query.OrderBy(l => l.Status),
            ("status", "desc") => query.OrderByDescending(l => l.Status),
            ("email", "asc") => query.OrderBy(l => l.Email),
            ("email", "desc") => query.OrderByDescending(l => l.Email),
            ("name", "desc") => query.OrderByDescending(l => l.Name),
            _ => query.OrderBy(l => l.Name)   // default: Name asc
        };

        // Page
        var leads = await query
            .Skip(req.Start)
            .Take(req.Length)
            .ToListAsync(cancellationToken);

        // Fetch agent names for this page only
        var agentIds = leads.Select(l => l.AssignedAgentId).Distinct().ToList();
        var agents = await _context.Users
            .AsNoTracking()
            .IgnoreQueryFilters()
            .Where(u => agentIds.Contains(u.Id))
            .ToDictionaryAsync(u => u.Id, u => u.FullName, cancellationToken);

        // Map to row DTO
        var rows = leads.Select(l => new LeadTableRowDto
        {
            Id = l.Id,
            Name = l.Name,
            Status = l.Status.ToString(),
            PhoneNumber = l.PhoneNumber,
            Email = l.Email,
            AssignedAgent = agents.TryGetValue(l.AssignedAgentId, out var name)
                            ? name : $"Agent #{l.AssignedAgentId}"
        }).ToList();

        return new DataTableResponseDto<LeadTableRowDto>
        {
            Draw = req.Draw,
            RecordsTotal = total,
            RecordsFiltered = filtered,
            Data = rows
        };
    }
}
