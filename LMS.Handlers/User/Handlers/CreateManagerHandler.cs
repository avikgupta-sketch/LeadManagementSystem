using LMS.Models.Entities;
using LMS.Models.DTOs.User;
using MediatR;
using Microsoft.AspNetCore.Identity;
using LMS.Handlers.Users.Commands;

namespace LMS.Handlers.Users.Handlers;

public class CreateManagerHandler : IRequestHandler<CreateManagerCommand, bool>
{
    private readonly UserManager<ApplicationUser> _userManager;

    public CreateManagerHandler(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<bool> Handle(CreateManagerCommand request, CancellationToken cancellationToken)
    {
        var dto = request.Dto;

        var user = new ApplicationUser
        {
            FullName = dto.FullName,
            Email = dto.Email,
            UserName = dto.Email,
            EmailConfirmed = true
        };

        var result = await _userManager.CreateAsync(user, dto.Password);

        if (!result.Succeeded)
            return false;

        await _userManager.AddToRoleAsync(user, "Manager");

        return true;
    }
}
