using LMS.Models.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using LMS.Handlers.Auth.Commands;

namespace LMS.Handlers.Auth.Handlers;

public class LoginHandler : IRequestHandler<LoginCommand, bool>
{
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly UserManager<ApplicationUser> _userManager;

    public LoginHandler(
        SignInManager<ApplicationUser> signInManager,
        UserManager<ApplicationUser> userManager)
    {
        _signInManager = signInManager;
        _userManager = userManager;
    }

    public async Task<bool> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var dto = request.LoginDto;

        var user = await _userManager.FindByEmailAsync(dto.Email);

        if (user == null || user.IsDeleted)
            return false;

        var result = await _signInManager.PasswordSignInAsync(
            user,
            dto.Password,
            dto.RememberMe,
            lockoutOnFailure: false);

        return result.Succeeded;
    }
}