using MediatR;

namespace LMS.Handlers.Users.Commands;

public class SoftDeleteUserCommand : IRequest<bool>
{
    public int UserId { get; set; }
    public int RequestedById { get; set; }

    public SoftDeleteUserCommand(int userId, int requestedById)
    {
        UserId = userId;
        RequestedById = requestedById;
    }
}
