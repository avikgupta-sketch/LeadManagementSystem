using LMS.Models.DTOs.User;
using MediatR;

namespace LMS.Handlers.Users.Commands;

public class CreateManagerCommand : IRequest<bool>
{
    public CreateManagerDto Dto { get; set; }

    public CreateManagerCommand(CreateManagerDto dto)
    {
        Dto = dto;
    }
}
