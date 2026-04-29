using LMS.Handlers.Leads.Queries;
using LMS.Handlers.Users.Commands;
using LMS.Models.DTOs.User;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

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
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteManager(int id)
    {
        int adminId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

        await _mediator.Send(new SoftDeleteUserCommand(id, adminId));

        return RedirectToAction("Managers");
    }
    [HttpPost]
    [Authorize(Roles = "Manager")]
    public async Task<IActionResult> DeleteAgent(int id)
    {
        int managerId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

        await _mediator.Send(new SoftDeleteUserCommand(id, managerId));

        return RedirectToAction("Agents");
    }

    

}