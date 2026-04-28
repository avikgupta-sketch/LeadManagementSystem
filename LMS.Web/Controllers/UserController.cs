using LMS.Models.DTOs.User;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using LMS.Handlers.Users.Commands;
using Microsoft.AspNetCore.Authorization;

namespace LMS.Web.Controllers;


public class UserController : Controller
{
    private readonly IMediator _mediator;

    public UserController(IMediator mediator)
    {
        _mediator = mediator;
    }
    [Authorize(Roles = "Admin")]
    public IActionResult Managers()
    {
        
        return View();
    }
    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> CreateManager(CreateManagerDto dto)
    {
        var result = await _mediator.Send(new CreateManagerCommand(dto));

        if (!result)
        {
            TempData["Error"] = "Failed to create manager";
        }

        return RedirectToAction("Managers");
    }
    [Authorize(Roles = "Manager")]
    public IActionResult Agents()
    {
        return View();
    }

    [HttpPost]
    [Authorize(Roles = "Manager")]
    public async Task<IActionResult> CreateAgent(CreateAgentDto dto)
    {
        int managerId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier).Value);

        var result = await _mediator.Send(new CreateAgentCommand(dto, managerId));

        if (!result)
        {
            TempData["Error"] = "Failed to create agent";
        }

        return RedirectToAction("Agents");
    }
}