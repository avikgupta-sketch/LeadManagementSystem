using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LMS.Models.DTOs.Dashboard;

namespace LMS.Web.Controllers;

[Authorize(Roles = "Manager,Agent")]
public class DashboardController : Controller
{
    private readonly IMediator _mediator;

    public DashboardController(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task<IActionResult> Index()
    {
        int userId = int.Parse(
            User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        if (User.IsInRole("Agent"))
        {
            DashboardDto data = await _mediator.Send(
                new GetAgentDashboardQuery(userId));

            return View(data);
        }

        if (User.IsInRole("Manager"))
        {
            DashboardDto data = await _mediator.Send(
                new GetManagerDashboardQuery(userId));

            return View(data);
        }

        
        return RedirectToAction("Managers", "User");
    }
}