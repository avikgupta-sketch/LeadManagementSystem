using LMS.Models.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using LMS.Handlers.Auth.Commands;

namespace LMS.Handlers.Auth.Handlers;

public class LogoutHandler : IRequestHandler<LogoutCommand>
{
    private readonly SignInManager<ApplicationUser> _signInManager;

    public LogoutHandler(SignInManager<ApplicationUser> signInManager)
    {
        _signInManager = signInManager;
    }

    public async Task Handle(LogoutCommand request, CancellationToken cancellationToken)
    {
        await _signInManager.SignOutAsync();
    }
}