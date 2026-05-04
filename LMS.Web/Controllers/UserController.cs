using LMS.Handlers.Users.Commands;
using LMS.Handlers.Users.Queries;
using LMS.Models.DTOs.User;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LMS.Web.Controllers;

[Authorize]
public class UserController : Controller
{
    private readonly IMediator _mediator;

    public UserController(IMediator mediator)
    {
        _mediator = mediator;
    }

    private int CurrentUserId() =>
        int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

    

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Managers()
    {
        var managers = await _mediator.Send(new GetUsersByRoleQuery("Manager"));
        return View(managers);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CreateManager(CreateManagerDto dto)
    {
        if (!ModelState.IsValid ||
            string.IsNullOrWhiteSpace(dto.FullName) ||
            string.IsNullOrWhiteSpace(dto.Email) ||
            string.IsNullOrWhiteSpace(dto.Password))
        {
            TempData["Error"] = "All fields are required";
            return RedirectToAction("Managers");
        }

        var result = await _mediator.Send(new CreateManagerCommand(dto));
        if (!result)
            TempData["Error"] = "Failed to create manager (email may already exist or password is too weak)";

        return RedirectToAction("Managers");
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> EditManager(int id)
    {
        var user = await _mediator.Send(new GetUserByIdQuery(id));
        if (user == null) return NotFound();
        return View(user);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> EditManager(EditUserDto dto)
    {
        if (!ModelState.IsValid)
            return View(dto);

        var ok = await _mediator.Send(new EditUserCommand(dto, CurrentUserId()));
        if (!ok)
        {
            TempData["Error"] = "Failed to update manager";
            return View(dto);
        }
        return RedirectToAction("Managers");
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteManager(int id)
    {
        var result = await _mediator.Send(new SoftDeleteUserCommand(id, CurrentUserId()));
        if (!result.Success)
            TempData["Error"] = result.Error;
        return RedirectToAction("Managers");
    }

    //  MANAGER: AGENTS 

    [Authorize(Roles = "Manager")]
    public async Task<IActionResult> Agents()
    {
        int managerId = CurrentUserId();
        var agents = await _mediator.Send(new GetUsersByRoleQuery("Agent", managerId));
        return View(agents);
    }

    [HttpPost]
    [Authorize(Roles = "Manager")]
    public async Task<IActionResult> CreateAgent(CreateAgentDto dto)
    {
        if (!ModelState.IsValid ||
            string.IsNullOrWhiteSpace(dto.FullName) ||
            string.IsNullOrWhiteSpace(dto.Email) ||
            string.IsNullOrWhiteSpace(dto.Password))
        {
            TempData["Error"] = "All fields are required";
            return RedirectToAction("Agents");
        }

        var result = await _mediator.Send(new CreateAgentCommand(dto, CurrentUserId()));
        if (!result)
            TempData["Error"] = "Failed to create agent (email may already exist or password is too weak)";

        return RedirectToAction("Agents");
    }

    [HttpGet]
    [Authorize(Roles = "Manager")]
    public async Task<IActionResult> EditAgent(int id)
    {
        var user = await _mediator.Send(new GetUserByIdQuery(id));
        if (user == null) return NotFound();
        return View(user);
    }

    [HttpPost]
    [Authorize(Roles = "Manager")]
    public async Task<IActionResult> EditAgent(EditUserDto dto)
    {
        if (!ModelState.IsValid)
            return View(dto);

        var ok = await _mediator.Send(new EditUserCommand(dto, CurrentUserId()));
        if (!ok)
        {
            TempData["Error"] = "Failed to update agent";
            return View(dto);
        }
        return RedirectToAction("Agents");
    }

    [HttpPost]
    [Authorize(Roles = "Manager")]
    public async Task<IActionResult> DeleteAgent(int id)
    {
        var result = await _mediator.Send(new SoftDeleteUserCommand(id, CurrentUserId()));
        if (!result.Success)
            TempData["Error"] = result.Error;
        return RedirectToAction("Agents");
    }
}
