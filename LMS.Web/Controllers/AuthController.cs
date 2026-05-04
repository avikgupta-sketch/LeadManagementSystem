using LMS.Models.DTOs.Auth;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using LMS.Handlers.Auth.Commands;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using LMS.Models.Entities;

namespace LMS.Web.Controllers;

public class AuthController : Controller
{
    private readonly IMediator _mediator;
    private readonly UserManager<ApplicationUser> _userManager;

    public AuthController(IMediator mediator, UserManager<ApplicationUser> userManager)
    {
        _mediator = mediator;
        _userManager = userManager;
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult Login()
    {
        if (User.Identity != null && User.Identity.IsAuthenticated)
        {
            if (User.IsInRole("Admin"))
                return RedirectToAction("Managers", "User");

            if (User.IsInRole("Manager"))
                return RedirectToAction("Agents", "User");

            return RedirectToAction("MyLeads", "Lead");
        }

        return View();
        
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginDto dto)
    {
        if (!ModelState.IsValid)
            return View(dto);

        var result = await _mediator.Send(new LoginCommand(dto));

        if (!result)
        {
            ModelState.AddModelError("", "Invalid credentials");
            return View(dto);
        }

        var user = await _userManager.FindByEmailAsync(dto.Email);
        var roles = await _userManager.GetRolesAsync(user);

        // Role-based redirect
        if (roles.Contains("Admin"))
            return RedirectToAction("Managers", "User");

        if (roles.Contains("Manager"))
            return RedirectToAction("Agents", "User");

        return RedirectToAction("MyLeads", "Lead");
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize]
    public async Task<IActionResult> Logout()
    {
        await _mediator.Send(new LogoutCommand());
        return RedirectToAction("Login");
    }
}
