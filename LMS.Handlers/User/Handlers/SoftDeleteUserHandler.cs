using LMS.Data.Context;
using LMS.Handlers.Users.Commands;
using LMS.Models.DTOs.Common;
using LMS.Models.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace LMS.Handlers.Users.Handlers;

public class SoftDeleteUserHandler : IRequestHandler<SoftDeleteUserCommand, OperationResult>
{
    private readonly AppDbContext _context;

    // Open / active statuses — anything not yet finalized.
    private static readonly LeadStatus[] OpenStatuses =
    {
        LeadStatus.New,
        LeadStatus.InProgress,
        LeadStatus.FollowUp,
        LeadStatus.Interested
    };

    public SoftDeleteUserHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<OperationResult> Handle(
        SoftDeleteUserCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken);

            if (user == null)
                return OperationResult.Fail("User not found.");

            var requester = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == request.RequestedById, cancellationToken);

            if (requester == null)
                return OperationResult.Fail("Requester not found.");

            // ───────────────────────────────────────────────────────────
            // ADMIN deleting a MANAGER  →  3-level rule
            // ───────────────────────────────────────────────────────────
            if (requester.ManagerId == null && user.ManagerId == null)
            {
                // Level 1: any of the manager's agents have OPEN leads → block
                bool agentsHaveOpenLeads = await _context.Leads
                    .AnyAsync(l => l.ManagerId == user.Id
                                   && OpenStatuses.Contains(l.Status), cancellationToken);

                if (agentsHaveOpenLeads)
                {
                    return OperationResult.Fail(
                        "Agents have open leads. Close or reassign all open leads first.");
                }

                // Level 2: manager still has any active (non-deleted) agents → block
                bool hasActiveAgents = await _context.Users
                    .AnyAsync(u => u.ManagerId == user.Id, cancellationToken);

                if (hasActiveAgents)
                {
                    return OperationResult.Fail(
                        "Manager still has active agents. Delete the agents first.");
                }

                // Level 3: clean → allow
                user.IsDeleted = true;
                await _context.SaveChangesAsync(cancellationToken);
                return OperationResult.Ok();
            }

            // ───────────────────────────────────────────────────────────
            // MANAGER deleting an AGENT  →  block if open leads
            // ───────────────────────────────────────────────────────────
            if (user.ManagerId == requester.Id)
            {
                bool hasOpenLeads = await _context.Leads
                    .AnyAsync(l => l.AssignedAgentId == user.Id
                                   && OpenStatuses.Contains(l.Status), cancellationToken);

                if (hasOpenLeads)
                {
                    return OperationResult.Fail(
                        "Cannot delete agent. Reassign or close all open leads first.");
                }

                user.IsDeleted = true;
                await _context.SaveChangesAsync(cancellationToken);
                return OperationResult.Ok();
            }

            return OperationResult.Fail("You are not authorized to delete this user.");
        }
        catch (Exception ex)
        {
            Log.Error(ex, "SoftDeleteUserHandler failed for UserId={UserId}", request.UserId);
            return OperationResult.Fail("An unexpected error occurred while deleting the user.");
        }
    }
}
