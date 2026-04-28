using MediatR;

namespace LMS.Handlers.Users.Queries;

public class GetAgentsByManagerQuery : IRequest<List<(int Id, string Name)>>
{
    public int ManagerId { get; set; }

    public GetAgentsByManagerQuery(int managerId)
    {
        ManagerId = managerId;
    }
}
