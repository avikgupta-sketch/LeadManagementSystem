using LMS.Handlers.Leads.Commands;
using LMS.Handlers.Leads.Queries;
using LMS.Handlers.Users.Queries;
using LMS.Models.DTOs.Lead;
using LMS.Models.Entities;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LMS.Web.Controllers;

[Authorize]
public class LeadController : Controller
{
    private readonly IMediator _mediator;
    private readonly UserManager<ApplicationUser> _userManager;
    public LeadController(
        IMediator mediator,
        UserManager<ApplicationUser> userManager)
    {
        _mediator = mediator;
        _userManager = userManager;
    }

    [Authorize(Roles = "Manager,Agent")]
    public async Task<IActionResult> Create()
    {
        int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

        
        if (User.IsInRole("Manager"))
        {
            var agents = await _mediator.Send(new GetAgentsByManagerQuery(userId));
            ViewBag.Agents = agents;
        }
        return View();
    }

    [HttpPost]
    [Authorize(Roles = "Manager,Agent")]
    [HttpPost]
    [Authorize(Roles = "Manager,Agent")]
    public async Task<IActionResult> Create(CreateLeadDto dto)
    {
        int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

        int managerId = userId;

        // 🔥 If Agent → override values
        if (User.IsInRole("Agent"))
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());

            if (user == null || user.ManagerId == null)
                return Unauthorized();

            managerId = user.ManagerId.Value;

            // Agent always assigns to self
            dto.AssignedAgentId = userId;
        }

        var result = await _mediator.Send(
            new CreateLeadCommand(dto, userId, managerId));

        if (!result)
        {
            TempData["Error"] = "Invalid agent selection";
        }

        return RedirectToAction(User.IsInRole("Agent") ? "MyLeads" : "Create");
    }
    [Authorize(Roles = "Agent")]
    public async Task<IActionResult> MyLeads()
    {
        int agentId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

        var leads = await _mediator.Send(new GetLeadsByAgentQuery(agentId));

        return View(leads);
    }
    [HttpPost]
    [Authorize(Roles = "Agent")]
    public async Task<IActionResult> UpdateStatus(UpdateLeadStatusDto dto)
    {
        int agentId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

        var result = await _mediator.Send(
            new UpdateLeadStatusCommand(dto, agentId));

        return RedirectToAction("MyLeads");
    }
    [HttpPost]
    [Authorize(Roles = "Manager")]
    public async Task<IActionResult> Reassign(ReassignLeadDto dto)
    {
        int managerId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

        var result = await _mediator.Send(
            new ReassignLeadCommand(dto, managerId));

        return RedirectToAction("Index"); // later we’ll improve
    }
    [HttpPost]
    [Authorize(Roles = "Manager")]
    public async Task<IActionResult> Delete(int id)
    {
        int managerId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

        await _mediator.Send(new SoftDeleteLeadCommand(id, managerId));

        return RedirectToAction("Index");
    }
    [Authorize]
    public async Task<IActionResult> Detail(int id)
    {
        var lead = await _mediator.Send(new GetLeadByIdQuery(id));

        if (lead == null)
            return NotFound();

        return View(lead);
    }
    [Authorize(Roles = "Manager")]
    public async Task<IActionResult> Index()
    {
        int managerId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

        var leads = await _mediator.Send(new GetLeadsByManagerQuery(managerId));

        return View(leads);
    }
}
