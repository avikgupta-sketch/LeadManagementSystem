using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MediatR;
using LMS.Models.DTOs.Dashboard;

public class GetAgentDashboardQuery : IRequest<DashboardDto>
{
    public int AgentId { get; set; }

    public GetAgentDashboardQuery(int agentId)
    {
        AgentId = agentId;
    }
}