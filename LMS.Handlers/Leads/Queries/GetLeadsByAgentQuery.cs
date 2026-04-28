using MediatR;
using LMS.Models.Entities;

namespace LMS.Handlers.Leads.Queries;

public class GetLeadsByAgentQuery : IRequest<List<Lead>>
{
    public int AgentId { get; set; }

    public GetLeadsByAgentQuery(int agentId)
    {
        AgentId = agentId;
    }
}
