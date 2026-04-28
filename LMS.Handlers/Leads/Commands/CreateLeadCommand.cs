using LMS.Models.DTOs.Lead;
using MediatR;

namespace LMS.Handlers.Leads.Commands;

public class CreateLeadCommand : IRequest<bool>
{
    public CreateLeadDto Dto { get; set; }
    public int CreatedById { get; set; }
    public int ManagerId { get; set; }

    public CreateLeadCommand(CreateLeadDto dto, int createdById, int managerId)
    {
        Dto = dto;
        CreatedById = createdById;
        ManagerId = managerId;
    }
}
