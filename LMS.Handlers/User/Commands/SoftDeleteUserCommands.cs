using LMS.Models.DTOs.Common;
using MediatR;

namespace LMS.Handlers.Users.Commands;

public class SoftDeleteUserCommand : IRequest<OperationResult>
{
    public int UserId { get; set; }
    public int RequestedById { get; set; }

    public SoftDeleteUserCommand(int userId, int requestedById)
    {
        UserId = userId;
        RequestedById = requestedById;
    }
}
