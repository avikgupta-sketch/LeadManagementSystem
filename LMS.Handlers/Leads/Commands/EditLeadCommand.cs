using LMS.Models.DTOs.Lead;
using MediatR;

namespace LMS.Handlers.Leads.Commands;

public class EditLeadCommand : IRequest<bool>
{
    public EditLeadDto Dto { get; set; }
    public int RequestedById { get; set; }
    public bool IsManager { get; set; }

    public EditLeadCommand(EditLeadDto dto, int requestedById, bool isManager)
    {
        Dto = dto;
        RequestedById = requestedById;
        IsManager = isManager;
    }
}
