using LMS.Models.DTOs.User;
using MediatR;

namespace LMS.Handlers.Users.Queries;

public class GetUsersByRoleQuery : IRequest<List<EditUserDto>>
{
    public string Role { get; set; }
    public int? ManagerId { get; set; }   // only used when Role == "Agent"

    public GetUsersByRoleQuery(string role, int? managerId = null)
    {
        Role = role;
        ManagerId = managerId;
    }
}
