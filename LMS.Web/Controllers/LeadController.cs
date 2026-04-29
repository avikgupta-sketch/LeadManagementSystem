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

    public LeadController(IMediator mediator, UserManager<ApplicationUser> userManager)
    {
        _mediator = mediator;
        _userManager = userManager;
    }

    private int CurrentUserId() =>
        int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

    // ---------- CREATE ----------

    [HttpGet]
    [Authorize(Roles = "Manager,Agent")]
    public async Task<IActionResult> Create()
    {
        if (User.IsInRole("Manager"))
        {
            var agents = await _mediator.Send(new GetAgentsByManagerQuery(CurrentUserId()));
            ViewBag.Agents = agents;
        }
        else
        {
            // Agent doesn't pick an agent — provide an empty list so the view never crashes.
            ViewBag.Agents = new List<(int, string)>();
        }

        return View(new CreateLeadDto());
    }

    [HttpPost]
    [Authorize(Roles = "Manager,Agent")]
    public async Task<IActionResult> Create(CreateLeadDto dto)
    {
        int userId = CurrentUserId();
        int managerId = userId;

        // 🔥 Agent → auto-assign to self, find their manager
        if (User.IsInRole("Agent"))
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null || user.ManagerId == null)
                return Unauthorized();

            managerId = user.ManagerId.Value;
            dto.AssignedAgentId = userId;
        }

        if (string.IsNullOrWhiteSpace(dto.Title) || string.IsNullOrWhiteSpace(dto.Description))
        {
            TempData["Error"] = "Title and Description are required";
            return RedirectToAction("Create");
        }

        var ok = await _mediator.Send(new CreateLeadCommand(dto, userId, managerId));

        if (!ok)
        {
            TempData["Error"] = User.IsInRole("Agent")
                ? "Could not create lead. Please try again or contact your manager."
                : "Could not create lead. Make sure the selected agent belongs to your team.";
        }

        return RedirectToAction(User.IsInRole("Agent") ? "MyLeads" : "Index");
    }

    // ---------- AGENT VIEW ----------

    [Authorize(Roles = "Agent")]
    public async Task<IActionResult> MyLeads()
    {
        var leads = await _mediator.Send(new GetLeadsByAgentQuery(CurrentUserId()));
        return View(leads);
    }

    // ---------- MANAGER VIEW ----------

    [Authorize(Roles = "Manager")]
    public async Task<IActionResult> Index()
    {
        int managerId = CurrentUserId();
        var leads = await _mediator.Send(new GetLeadsByManagerQuery(managerId));

        // Provide agent dropdown for the inline reassign form
        var agents = await _mediator.Send(new GetAgentsByManagerQuery(managerId));
        ViewBag.Agents = agents;

        return View(leads);
    }

    // ---------- DETAIL (all roles) ----------

    [Authorize]
    public async Task<IActionResult> Detail(int id)
    {
        var lead = await _mediator.Send(new GetLeadByIdQuery(id));
        if (lead == null)
            return NotFound();
        return View(lead);
    }

    // ---------- EDIT (Manager OR Agent) ----------

    [HttpGet]
    [Authorize(Roles = "Manager,Agent")]
    public async Task<IActionResult> Edit(int id)
    {
        var lead = await _mediator.Send(new GetLeadByIdQuery(id));
        if (lead == null)
            return NotFound();

        var dto = new EditLeadDto
        {
            Id = lead.Id,
            Title = lead.Title,
            Description = lead.Description
        };

        ViewBag.Status = lead.Status;
        return View(dto);
    }

    [HttpPost]
    [Authorize(Roles = "Manager,Agent")]
    public async Task<IActionResult> Edit(EditLeadDto dto)
    {
        if (!ModelState.IsValid)
            return View(dto);

        bool isManager = User.IsInRole("Manager");
        var ok = await _mediator.Send(new EditLeadCommand(dto, CurrentUserId(), isManager));

        if (!ok)
        {
            TempData["Error"] = "Could not update lead. " +
                                "It may be in a terminal status (Converted/Closed/Rejected) " +
                                "or you may not have permission.";
            return View(dto);
        }

        return RedirectToAction(isManager ? "Index" : "MyLeads");
    }

    // ---------- STATUS UPDATE (Agent only) ----------

    [HttpPost]
    [Authorize(Roles = "Agent")]
    public async Task<IActionResult> UpdateStatus(UpdateLeadStatusDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Remark))
            TempData["Error"] = "Remark is required to change status";
        else
        {
            var ok = await _mediator.Send(new UpdateLeadStatusCommand(dto, CurrentUserId()));
            if (!ok)
                TempData["Error"] = "Status update failed (lead may already be Converted/Closed/Rejected).";
        }

        return RedirectToAction("MyLeads");
    }

    // ---------- REASSIGN (Manager only) ----------

    [HttpPost]
    [Authorize(Roles = "Manager")]
    public async Task<IActionResult> Reassign(ReassignLeadDto dto)
    {
        var ok = await _mediator.Send(new ReassignLeadCommand(dto, CurrentUserId()));
        if (!ok)
            TempData["Error"] = "Could not reassign. Lead may be terminal-status, or agent may not be in your team.";

        return RedirectToAction("Index");
    }

    // ---------- DELETE (Manager only) ----------

    [HttpPost]
    [Authorize(Roles = "Manager")]
    public async Task<IActionResult> Delete(int id)
    {
        await _mediator.Send(new SoftDeleteLeadCommand(id, CurrentUserId()));
        return RedirectToAction("Index");
    }
}
