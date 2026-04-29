using LMS.Models.DTOs.User;
using MediatR;

namespace LMS.Handlers.Users.Commands;

public class EditUserCommand : IRequest<bool>
{
    public EditUserDto Dto { get; set; }
    public int RequestedById { get; set; }

    public EditUserCommand(EditUserDto dto, int requestedById)
    {
        Dto = dto;
        RequestedById = requestedById;
    }
}
