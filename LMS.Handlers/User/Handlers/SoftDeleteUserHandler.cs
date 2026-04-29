using LMS.Data.Context;
using LMS.Handlers.Users.Commands;
using LMS.Models.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;


namespace LMS.Handlers.Users.Handlers;

public class SoftDeleteUserHandler : IRequestHandler<SoftDeleteUserCommand, bool>
{
    private readonly AppDbContext _context;

    public SoftDeleteUserHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(SoftDeleteUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == request.UserId);

        if (user == null)
            return false;

        // 🔴 Get requester (who is performing delete)
        var requester = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == request.RequestedById);

        if (requester == null)
            return false;

        // 🔴 Admin can delete Manager
        if (requester.ManagerId == null) // Admin has no ManagerId
        {
            user.IsDeleted = true;
            await _context.SaveChangesAsync();
            return true;
        }

        // 🔴 Manager can delete only their Agents
        if (user.ManagerId == requester.Id)
        {
            user.IsDeleted = true;
            await _context.SaveChangesAsync();
            return true;
        }

        return false;
    }
}