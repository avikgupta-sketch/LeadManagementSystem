using LMS.Handlers.Users.Queries;
using LMS.Models.DTOs.User;
using LMS.Models.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace LMS.Handlers.Users.Handlers;

public class GetUsersByRoleHandler : IRequestHandler<GetUsersByRoleQuery, List<EditUserDto>>
{
    private readonly UserManager<ApplicationUser> _userManager;

    public GetUsersByRoleHandler(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<List<EditUserDto>> Handle(GetUsersByRoleQuery request, CancellationToken cancellationToken)
    {
        var usersInRole = await _userManager.GetUsersInRoleAsync(request.Role);

        // global query filter on ApplicationUser already excludes IsDeleted == true,
        // but GetUsersInRoleAsync runs its own query so re-filter here for safety.
        IEnumerable<ApplicationUser> filtered = usersInRole.Where(u => !u.IsDeleted);

        if (request.Role == "Agent" && request.ManagerId.HasValue)
        {
            filtered = filtered.Where(u => u.ManagerId == request.ManagerId.Value);
        }

        return filtered
            .OrderBy(u => u.FullName)
            .Select(u => new EditUserDto
            {
                Id = u.Id,
                FullName = u.FullName ?? string.Empty,
                Email = u.Email ?? string.Empty
            })
            .ToList();
    }
}
