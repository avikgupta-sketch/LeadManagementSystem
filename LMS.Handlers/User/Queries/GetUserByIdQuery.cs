using LMS.Models.DTOs.User;
using MediatR;

namespace LMS.Handlers.Users.Queries;

public class GetUserByIdQuery : IRequest<EditUserDto?>
{
    public int UserId { get; set; }

    public GetUserByIdQuery(int userId)
    {
        UserId = userId;
    }
}
