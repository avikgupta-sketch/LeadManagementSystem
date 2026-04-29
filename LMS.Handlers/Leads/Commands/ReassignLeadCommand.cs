
using LMS.Models.DTOs.Lead;
using MediatR;

namespace LMS.Handlers.Leads.Commands;

public class ReassignLeadCommand : IRequest<bool>
{
    public ReassignLeadDto Dto { get; set; }
    public int ManagerId { get; set; }

    public ReassignLeadCommand(ReassignLeadDto dto, int managerId)
    {
        Dto = dto;
        ManagerId = managerId;
    }
}