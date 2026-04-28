using LMS.Models.DTOs.User;
using MediatR;

namespace LMS.Handlers.Users.Commands;

public class CreateAgentCommand : IRequest<bool>
{
    public CreateAgentDto Dto { get; set; }
    public int ManagerId { get; set; }

    public CreateAgentCommand(CreateAgentDto dto, int managerId)
    {
        Dto = dto;
        ManagerId = managerId;
    }
}