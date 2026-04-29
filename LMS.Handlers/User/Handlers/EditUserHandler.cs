using LMS.Handlers.Users.Commands;
using LMS.Models.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace LMS.Handlers.Users.Handlers;

public class EditUserHandler : IRequestHandler<EditUserCommand, bool>
{
    private readonly UserManager<ApplicationUser> _userManager;

    public EditUserHandler(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<bool> Handle(EditUserCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var dto = request.Dto;

            var user = await _userManager.Users
                .FirstOrDefaultAsync(u => u.Id == dto.Id && !u.IsDeleted, cancellationToken);

            if (user == null)
                return false;

            // 🔴 Authorization: requester must be Admin (manager edit) OR
            //    requester must be the manager of the agent being edited.
            var requester = await _userManager.Users
                .FirstOrDefaultAsync(u => u.Id == request.RequestedById, cancellationToken);

            if (requester == null)
                return false;

            // Admin (no ManagerId) can edit Managers (users whose ManagerId == null and not the admin himself)
            // Manager can edit only their own Agents (user.ManagerId == requester.Id)
            bool requesterIsAdmin = requester.ManagerId == null
                                    && (await _userManager.IsInRoleAsync(requester, "Admin"));

            bool isAdminEditingManager = requesterIsAdmin
                                         && (await _userManager.IsInRoleAsync(user, "Manager"));

            bool isManagerEditingOwnAgent = !requesterIsAdmin
                                             && user.ManagerId == requester.Id;

            if (!isAdminEditingManager && !isManagerEditingOwnAgent)
                return false;

            // 🔥 Update only FullName + Email (NEVER password — per requirements)
            user.FullName = dto.FullName;
            user.Email = dto.Email;
            user.UserName = dto.Email;
            user.NormalizedEmail = _userManager.NormalizeEmail(dto.Email);
            user.NormalizedUserName = _userManager.NormalizeName(dto.Email);

            var result = await _userManager.UpdateAsync(user);
            return result.Succeeded;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "EditUserHandler failed for UserId={UserId}", request.Dto.Id);
            return false;
        }
    }
}
