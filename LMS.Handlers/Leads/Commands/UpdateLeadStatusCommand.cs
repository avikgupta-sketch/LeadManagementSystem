using LMS.Models.DTOs.Lead;
using MediatR;

namespace LMS.Handlers.Leads.Commands;

public class UpdateLeadStatusCommand : IRequest<bool>
{
    public UpdateLeadStatusDto Dto { get; set; }
    public int AgentId { get; set; }

    public UpdateLeadStatusCommand(UpdateLeadStatusDto dto, int agentId)
    {
        Dto = dto;
        AgentId = agentId;
    }
}
