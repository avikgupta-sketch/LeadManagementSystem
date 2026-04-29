using MediatR;

namespace LMS.Handlers.Leads.Commands;

public class SoftDeleteLeadCommand : IRequest<bool>
{
    public int LeadId { get; set; }
    public int ManagerId { get; set; }

    public SoftDeleteLeadCommand(int leadId, int managerId)
    {
        LeadId = leadId;
        ManagerId = managerId;
    }
}