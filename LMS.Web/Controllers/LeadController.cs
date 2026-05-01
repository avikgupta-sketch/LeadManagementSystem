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

    // ─────────────────────────────────────────────────────────
    // CREATE
    // ─────────────────────────────────────────────────────────

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

        // Agent → auto-assign to self, find their manager
        if (User.IsInRole("Agent"))
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null || user.ManagerId == null)
                return Unauthorized();

            managerId = user.ManagerId.Value;
            dto.AssignedAgentId = userId;
        }

        if (!ModelState.IsValid)
        {
            if (User.IsInRole("Manager"))
            {
                var agents = await _mediator.Send(new GetAgentsByManagerQuery(userId));
                ViewBag.Agents = agents;
            }
            else
            {
                ViewBag.Agents = new List<(int, string)>();
            }
            return View(dto);
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

    // ─────────────────────────────────────────────────────────
    // LIST  — same shared view for Agent (MyLeads) & Manager (Index)
    // ─────────────────────────────────────────────────────────

    [Authorize(Roles = "Agent")]
    public async Task<IActionResult> MyLeads()
    {
        var leads = await _mediator.Send(new GetLeadsByAgentQuery(CurrentUserId()));
        return View("List", leads);
    }

    [Authorize(Roles = "Manager")]
    public async Task<IActionResult> Index()
    {
        var leads = await _mediator.Send(new GetLeadsByManagerQuery(CurrentUserId()));
        return View("List", leads);
    }

    // ─────────────────────────────────────────────────────────
    // DETAIL  — combined View + Edit page (all editable fields on the
    //          left, status/reassign panel on the right, audit log
    //          underneath). Both roles use the same view; the right
    //          panel switches based on role.
    // ─────────────────────────────────────────────────────────

    [Authorize(Roles = "Manager,Agent")]
    public async Task<IActionResult> Detail(int id)
    {
        var lead = await _mediator.Send(new GetLeadByIdQuery(id));
        if (lead == null)
            return NotFound();

        // For Manager: provide list of their agents for the Reassign dropdown
        if (User.IsInRole("Manager"))
        {
            var agents = await _mediator.Send(new GetAgentsByManagerQuery(CurrentUserId()));
            ViewBag.Agents = agents;
        }
        else
        {
            ViewBag.Agents = new List<(int, string)>();
        }

        return View(lead);
    }

    // POST coming from the "edit fields" form on the Detail page.
    [HttpPost]
    [Authorize(Roles = "Manager,Agent")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(EditLeadDto dto)
    {
        if (!ModelState.IsValid)
        {
            TempData["Error"] = "Please fix the highlighted errors and save again.";
            return RedirectToAction("Detail", new { id = dto.Id });
        }

        bool isManager = User.IsInRole("Manager");
        var ok = await _mediator.Send(new EditLeadCommand(dto, CurrentUserId(), isManager));

        if (!ok)
        {
            TempData["Error"] = "Could not update lead. " +
                                "It may be in a terminal status (Converted/Closed/Rejected) " +
                                "or you may not have permission.";
        }
        else
        {
            TempData["Success"] = "Lead updated successfully.";
        }

        return RedirectToAction("Detail", new { id = dto.Id });
    }

    // ─────────────────────────────────────────────────────────
    // STATUS UPDATE  (Agent only) — posted from Detail page
    // ─────────────────────────────────────────────────────────
    [HttpPost]
    [Authorize(Roles = "Agent")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateStatus(UpdateLeadStatusDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Remark))
        {
            TempData["Error"] = "Remark is required to change status";
        }
        else
        {
            var ok = await _mediator.Send(new UpdateLeadStatusCommand(dto, CurrentUserId()));
            if (!ok)
                TempData["Error"] = "Status update failed (lead may already be Converted/Closed/Rejected).";
            else
                TempData["Success"] = "Status updated.";
        }

        return RedirectToAction("Detail", new { id = dto.LeadId });
    }

    // ─────────────────────────────────────────────────────────
    // REASSIGN  (Manager only) — posted from Detail page
    // ─────────────────────────────────────────────────────────
    [HttpPost]
    [Authorize(Roles = "Manager")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Reassign(ReassignLeadDto dto)
    {
        var ok = await _mediator.Send(new ReassignLeadCommand(dto, CurrentUserId()));
        if (!ok)
            TempData["Error"] = "Could not reassign. Lead may be terminal-status, or agent may not be in your team.";
        else
            TempData["Success"] = "Lead reassigned.";

        return RedirectToAction("Detail", new { id = dto.LeadId });
    }

    // ─────────────────────────────────────────────────────────
    // DELETE  (Manager only)
    // ─────────────────────────────────────────────────────────
    [HttpPost]
    [Authorize(Roles = "Manager")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        await _mediator.Send(new SoftDeleteLeadCommand(id, CurrentUserId()));
        return RedirectToAction("Index");
    }
}