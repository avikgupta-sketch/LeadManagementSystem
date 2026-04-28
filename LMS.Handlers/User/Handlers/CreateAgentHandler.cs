using LMS.Models.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using LMS.Handlers.Users.Commands;

namespace LMS.Handlers.Users.Handlers;

public class CreateAgentHandler : IRequestHandler<CreateAgentCommand, bool>
{
    private readonly UserManager<ApplicationUser> _userManager;

    public CreateAgentHandler(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<bool> Handle(CreateAgentCommand request, CancellationToken cancellationToken)
    {
        var dto = request.Dto;

        var agent = new ApplicationUser
        {
            FullName = dto.FullName,
            Email = dto.Email,
            UserName = dto.Email,
            ManagerId = request.ManagerId,
            EmailConfirmed = true
        };

        var result = await _userManager.CreateAsync(agent, dto.Password);

        if (!result.Succeeded)
            return false;

        await _userManager.AddToRoleAsync(agent, "Agent");

        return true;
    }
}
